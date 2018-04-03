using Nancy.Bootstrapper;
using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.CacheStore;
using Nancy.Routing;

namespace Nancy.RapidCache.Extensions
{
    public static class NancyBootstrapperExtensions
    {
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
    }
}
