using Nancy.RapidCache.Extensions;
using Nancy.Routing;
using System;

namespace Nancy.RapidCache.Tests.Fakes
{
    public class FakeDefaultBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoc.TinyIoCContainer container, Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines);
            pipelines.AfterRequest.AddItemToStartOfPipeline(ConfigureCache);
        }

        public void ConfigureCache(NancyContext context)
        {
            if (context.Response.StatusCode == HttpStatusCode.OK)
            {
                context.Response = context.Response.AsCacheable(DateTime.UtcNow.AddSeconds(30));
            }
        }
    }
}
