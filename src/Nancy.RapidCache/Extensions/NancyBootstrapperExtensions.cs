using Nancy.Bootstrapper;
using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.CacheStore;
using Nancy.Routing;

namespace Nancy.RapidCache.Extensions
{
    public static class NancyBootstrapperExtensions
    {
        const string RemovalExceptionMessage = "Rapid Cache is disabled, enable to use the removal key";
        const string DisableExceptionMessage = "Rapid Cache is disabled, enable to use the disability key";

        /// <summary>
        /// Enables Nancy.RapidCache using default store and key generator
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"></param>
        public static void EnableRapidCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines)
            => RapidCache.Enable(bootstrapper, routeResolver, pipelines);

        /// <summary>
        /// Enables Nancy.RapidCache using the supplied parameters
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"></param>
        public static void EnableRapidCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, string[] varyParams)
            => RapidCache.Enable(bootstrapper, routeResolver, pipelines, varyParams);

        /// <summary>
        /// Enables Nancy.RapidCache using the supplied parameters and CacheStore type
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"></param>
        /// <param name="cacheStore"> </param>
        public static void EnableRapidCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, string[] varyParams, ICacheStore cacheStore)
            => RapidCache.Enable(bootstrapper, routeResolver, pipelines, new DefaultCacheKeyGenerator(varyParams), cacheStore);

        /// <summary>
        /// Enables Nancy.RapidCache using the supplied parameters
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="cacheKeyGenerator"></param>
        public static void EnableRapidCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, ICacheKeyGenerator cacheKeyGenerator)
            => RapidCache.Enable(bootstrapper, routeResolver, pipelines, cacheKeyGenerator);

        /// <summary>
        /// Enables Nancy.RapidCache using the supplied parameters and CacheStore type
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="cacheKeyGenerator"></param>
        /// <param name="cacheStore"> </param>
        public static void EnableRapidCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, ICacheKeyGenerator cacheKeyGenerator, ICacheStore cacheStore)
            => RapidCache.Enable(bootstrapper, routeResolver, pipelines, cacheKeyGenerator, cacheStore);

        /// <summary>
        /// Enable the option of removing cache keys by a request, this option is off by default for security measurements.
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="key"></param>
        public static void EnableCacheRemovalKey(this INancyBootstrapper bootstrapper, string key = null)
        {
            if (!RapidCache.IsCacheEnabled())
                throw new BootstrapperException(RemovalExceptionMessage);

#if NETSTANDARD2_0
            Defaults.RemoveCache.Enabled = true;

            if (!string.IsNullOrEmpty(key))
            {
                Defaults.RemoveCache.Key = key;
            }
#else
            Defaults.RemoveCacheEnabled = true;

            if (!string.IsNullOrEmpty(key))
            {
                Defaults.RemoveCacheKey = key;
            }
#endif
        }

        /// <summary>
        /// Enable the option of disability cache keys by a request, this option is off by default for security measurements.
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="key"></param>
        public static void EnableCacheDisableKey(this INancyBootstrapper bootstrapper, string key = null)
        {
            if (!RapidCache.IsCacheEnabled())
                throw new BootstrapperException(DisableExceptionMessage);

#if NETSTANDARD2_0

            Defaults.DisableCache.Enabled = true;

            if (!string.IsNullOrEmpty(key))
            {
                Defaults.DisableCache.Key = key;
            }
#else
            Defaults.NoRequestQueryEnabled = true;

            if (!string.IsNullOrEmpty(key))
            {
                Defaults.NoRequestQueryKey = key;
            }
#endif
        }
    }
}
