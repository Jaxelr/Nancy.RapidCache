using Nancy.Bootstrapper;
using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Projection;
using Nancy.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using static Nancy.RapidCache.Defaults;

namespace Nancy.RapidCache
{
    /// <summary>
    /// Asynchronous cache for Nancy
    /// </summary>
    public class RapidCache
    {
        private static bool _enabled;
        private static ICacheStore _cacheStore;
        private static ICacheKeyGenerator _cacheKeyGenerator;
        private static IRouteResolver _routeResolver;
        private static INancyBootstrapper _nancyBootstrapper;
        private static INancyEngine NancyEngine => _nancyBootstrapper.GetEngine();

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"> </param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver, IPipelines pipelines)
            => Enable(nancyBootstrapper, routeResolver, pipelines, new DefaultCacheKeyGenerator(), new MemoryCacheStore());

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"> </param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver, IPipelines pipelines, string[] varyParams)
            => Enable(nancyBootstrapper, routeResolver, pipelines, new DefaultCacheKeyGenerator(varyParams), new MemoryCacheStore());

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipelines"></param>
        /// <param name="cacheKeyGenerator"></param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver, IPipelines pipelines, ICacheKeyGenerator cacheKeyGenerator)
            => Enable(nancyBootstrapper, routeResolver, pipelines, cacheKeyGenerator, new MemoryCacheStore());

        /// <summary>
        ///
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipeline"></param>
        /// <param name="cacheKeyGenerator"></param>
        /// <param name="cacheStore"></param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver, IPipelines pipeline, ICacheKeyGenerator cacheKeyGenerator, ICacheStore cacheStore)
        {
            if (_enabled || cacheKeyGenerator is null || cacheStore is null)
            {
                return;
            }

            _enabled = true;
            _cacheKeyGenerator = cacheKeyGenerator;
            _cacheStore = cacheStore;
            _nancyBootstrapper = nancyBootstrapper;
            _routeResolver = routeResolver;
            pipeline.BeforeRequest.AddItemToStartOfPipeline(CheckCache);
            pipeline.AfterRequest.AddItemToEndOfPipeline(SetCache);
        }

        /// <summary>
        /// Returns the state of the Cache.
        /// Mostly used for discovery purposes since this is a static value.
        /// </summary>
        /// <returns></returns>
        public static bool IsCacheEnabled() => _enabled;

        /// <summary>
        /// Invokes pre-requirements such as authentication and stuff for the supplied context
        /// reference: https://github.com/NancyFx/Nancy/blob/master/src/Nancy/Routing/DefaultRequestDispatcher.cs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Response InvokePreRequirements(NancyContext context)
        {
            var resolution = _routeResolver.Resolve(context);
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
                if (dict.ContainsKey(NoRequestQueryKey))
                {
                    return null;
                }
            }

            string key = _cacheKeyGenerator.Get(context.Request);

            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var response = _cacheStore.Get(key);

            if (response == null || response.Expiration < DateTime.UtcNow)
            {
                return null;
            }

            new Thread(() => HandleRequest(context.Request, key))
                .Start();

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

            string key = _cacheKeyGenerator.Get(context.Request);

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (context.Response.StatusCode != HttpStatusCode.OK)
            {
                _cacheStore.Remove(key);
                return;
            }

            if (context.Response is CacheableResponse cacheableResponse)
            {
                var currentCache = _cacheStore.Get(key);

                if (currentCache == null || currentCache?.Expiration < DateTime.UtcNow)
                {
                    _cacheStore.Set(key, context, cacheableResponse.Expiration);
                }
            }
            else if (context.NegotiationContext.Headers.ContainsKey(CacheHeader.ToLowerInvariant()))
            {
                var expiration = DateTime.Parse(context.NegotiationContext.Headers[CacheHeader], CultureInfo.InvariantCulture);
                context.NegotiationContext.Headers.Remove(CacheHeader);
                _cacheStore.Set(key, context, expiration);
            }
        }

        private static readonly List<string> RequestSyncKeys = new List<string>();
        private static readonly object Lock = new object();

        /// <summary>
        /// used to asynchronously cache Nancy Requests
        /// </summary>
        /// <param name="context"></param>
        private static void HandleRequest(object context, string key)
        {
            lock (Lock)
            {

                if (!(context is Request request) || string.IsNullOrEmpty(key))
                {
                    return;
                }

                try
                {
                    if (RequestSyncKeys.Contains(key))
                    {
                        return;
                    }

                    RequestSyncKeys.Add(key);

                    request.Query[NoRequestQueryKey] = NoRequestQueryKey;

                    var context2 = NancyEngine.HandleRequest(request);

#if NETSTANDARD2_0
                    if (context2.Result.Response.StatusCode != HttpStatusCode.OK)
                    {
                        _cacheStore.Remove(key);
                    }
#else
                    if (context2.Response.StatusCode != HttpStatusCode.OK)
                    {
                        _cacheStore.Remove(key);
                    }
#endif
                }
                catch (Exception)
                {
                    _cacheStore.Remove(key);
                }
                finally
                {
                    RequestSyncKeys.Remove(key);
                }
            }
        }
    }
}
