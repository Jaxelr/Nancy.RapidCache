﻿using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.Extensions;
using Nancy.Routing;
using System;

namespace Nancy.RapidCache.Tests.Mocks
{
    public class MockMemoryBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoc.TinyIoCContainer container, Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new DefaultCacheKeyGenerator(new[] { "query", "form", "accept" }));
            pipelines.AfterRequest.AddItemToStartOfPipeline(ConfigureCache);
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
