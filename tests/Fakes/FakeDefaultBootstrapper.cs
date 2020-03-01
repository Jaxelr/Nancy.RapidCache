using Nancy.RapidCache.Extensions;
using Nancy.Routing;
using System;

namespace Nancy.RapidCache.Tests.Fakes
{
    public class FakeDefaultBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoc.TinyIoCContainer container, Bootstrapper.IPipelines pipelines)
        {
            string[] parameters = new[] { "query", "form", "accept" };

            base.ApplicationStartup(container, pipelines);

            //TODO: Since its complicated to test using multiple boostrappers (thanks to assembly scanning done by Nancy)
            //      We call all declarations here just to verify that they are not broken 
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines);
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, parameters);
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, parameters, new CacheStore.MemoryCacheStore());
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new CacheKey.DefaultCacheKeyGenerator(parameters));
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new CacheKey.DefaultCacheKeyGenerator(parameters), new CacheStore.MemoryCacheStore());

            pipelines.AfterRequest.AddItemToStartOfPipeline(ConfigureCache);
        }

        private void ConfigureCache(NancyContext context)
        {
            if (context.Response.StatusCode == HttpStatusCode.OK && (context.Request.Method == "GET" || context.Request.Method == "HEAD"))
            {
                context.Response = context.Response.AsCacheable(DateTime.UtcNow.AddSeconds(1));
            }
        }
    }
}
