using Nancy;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.Extensions;
using Nancy.Routing;
using System;

namespace Asp.Net.Example
{
    public class ApplicationBootrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            /*enable RapidCache, vary by url params query, form and accept headers */
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new DefaultCacheKeyGenerator(new[] { "query", "form", "accept" }));
            //pipelines.AfterRequest.AddItemToStartOfPipeline(ConfigureCache);

            /* Enable cache using the DiskCacheStore, vary by url query, form and accept headers */
            //this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new DefaultCacheKeyGenerator(new[] { "query", "form", "accept" }), new DiskCacheStore("c:/tmp/cache"));

            /* Enable cache using Redis , using the same key variations */
            //this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new DefaultCacheKeyGenerator(new[] { "query", "form", "accept" }), new RedisCacheStore("localhost"));
        }

        public void ConfigureCache(NancyContext context)
        {
            if (context.Response.StatusCode == HttpStatusCode.OK)
            {
                context.Response = context.Response.AsCacheable(DateTime.Now.AddSeconds(30));
            }
        }
    }
}
