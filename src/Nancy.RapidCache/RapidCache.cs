using System;
using System.Globalization;
using System.Threading;
using Nancy.Bootstrapper;
using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Projection;
using Nancy.Routing;
using static Nancy.RapidCache.Defaults;

namespace Nancy.RapidCache
{
    /// <summary>
    /// Asynchronous cache for Nancy
    /// </summary>
    public class RapidCache
    {
        private static bool enabled;
        private static ICacheStore cacheStore;
        private static ICacheKeyGenerator cacheKeyGenerator;
        private static IRouteResolver routeResolver;
        private static INancyBootstrapper nancyBootstrapper;

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"> </param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver,
            IPipelines pipelines)
            => Enable(nancyBootstrapper, routeResolver, pipelines, new DefaultCacheKeyGenerator(),
                new MemoryCacheStore());

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"> </param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver,
            IPipelines pipelines, string[] varyParams)
            => Enable(nancyBootstrapper, routeResolver, pipelines, new DefaultCacheKeyGenerator(varyParams),
                new MemoryCacheStore());

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipelines"></param>
        /// <param name="cacheKeyGenerator"></param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver,
            IPipelines pipelines, ICacheKeyGenerator cacheKeyGenerator)
            => Enable(nancyBootstrapper, routeResolver, pipelines, cacheKeyGenerator, new MemoryCacheStore());

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipeline"></param>
        /// <param name="cacheKeyGenerator"></param>
        /// <param name="cacheStore"></param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver,
            IPipelines pipeline, ICacheKeyGenerator cacheKeyGenerator, ICacheStore cacheStore)
        {
            if (enabled || cacheKeyGenerator is null || cacheStore is null)
            {
                return;
            }

            enabled = true;
            RapidCache.cacheKeyGenerator = cacheKeyGenerator;
            RapidCache.cacheStore = cacheStore;
            RapidCache.nancyBootstrapper = nancyBootstrapper;
            RapidCache.routeResolver = routeResolver;
            pipeline.BeforeRequest.AddItemToStartOfPipeline(CheckCache);
            pipeline.AfterRequest.AddItemToEndOfPipeline(SetCache);
        }

        /// <summary>
        /// Returns the state of the Cache.
        /// Mostly used for discovery purposes since this is a static value.
        /// </summary>
        /// <returns></returns>
        public static bool IsCacheEnabled() => enabled;

        /// <summary>
        /// Invokes pre-requirements such as authentication and stuff for the supplied context
        /// reference: https://github.com/NancyFx/Nancy/blob/master/src/Nancy/Routing/DefaultRequestDispatcher.cs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Response InvokePreRequirements(NancyContext context)
        {
            var resolution = routeResolver.Resolve(context);
            var preRequirements = resolution.Before;
            var task = preRequirements.Invoke(context, new CancellationToken(false));
            task.Wait();
            return context.Response;
        }

        /// <summary>
        /// checks current cachestore for a cached response and returns it
        /// </summary>
        /// <param name="context"></param>
        /// <returns>cached response or null</returns>
        private static Response CheckCache(NancyContext context)
        {
            if (context.Request.Query is DynamicDictionary dict)
            {
#if NETSTANDARD2_0
                if (DisableCache.Enabled && dict.ContainsKey(DisableCache.Key))
                {
                    return null;
                }
#else
                if (NoRequestQueryEnabled && dict.ContainsKey(NoRequestQueryKey))
                {
                    return null;
                }
#endif
            }

            string key = cacheKeyGenerator.Get(context.Request);

            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (context.Request.Query is DynamicDictionary rmv)
            {
#if NETSTANDARD2_0
                if (RemoveCache.Enabled && rmv.ContainsKey(RemoveCache.Key))
                {
                    cacheStore.Remove(key);
                    return null;
                }
#else
                if (RemoveCacheEnabled && rmv.ContainsKey(RemoveCacheKey))
                {
                    cacheStore.Remove(key);
                    return null;   
                }
#endif
            }

            var response = cacheStore.Get(key);

            if (response == null || response.Expiration < DateTime.UtcNow)
            {
                return null;
            }

            //make damn sure the pre-requirements are met before returning a cached response
            var preResponse = InvokePreRequirements(context);
            if (preResponse != null)
            {
                return preResponse;
            }

            return response;
        }

        /// <summary>
        /// caches response before it is sent to client if it is a CacheableResponse or if the NegotationContext has the nancy-rapidcache
        /// header set.
        /// </summary>
        /// <param name="context"></param>
        private static void SetCache(NancyContext context)
        {
            if (context.Response is CachedResponse)
            {
                return;
            }

            if (context.Request.Query is DynamicDictionary dict)
            {
#if NETSTANDARD2_0
                if (DisableCache.Enabled && dict.ContainsKey(DisableCache.Key))
                {
                    return;
                }
#else
                if (NoRequestQueryEnabled && dict.ContainsKey(NoRequestQueryKey))
                {
                    return;
                }
#endif
            }

            string key = cacheKeyGenerator.Get(context.Request);

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (context.Response.StatusCode != HttpStatusCode.OK)
            {
                cacheStore.Remove(key);
                return;
            }

            var currentCache = cacheStore.Get(key);
            var now = DateTime.UtcNow;

            if (context.Response is CacheableResponse cacheableResponse)
            {
                if (currentCache == null || currentCache?.Expiration < now)
                {
                    cacheStore.Set(key, context, cacheableResponse.Expiration);
                }
            }
            else if (context.NegotiationContext.Headers.ContainsKey(CacheHeader.ToLowerInvariant()))
            {
                var expiration = DateTime.Parse(context.NegotiationContext.Headers[CacheHeader],
                    CultureInfo.InvariantCulture);

                context.NegotiationContext.Headers.Remove(CacheHeader);

                if (currentCache == null || currentCache?.Expiration < now)
                {
                    cacheStore.Set(key, context, expiration);
                }
            }
        }
    }
}
