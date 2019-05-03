using Nancy;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Extensions;
using Nancy.Routing;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;

namespace Asp.Net.Example
{
    public class ApplicationBootrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            /*enable RapidCache, vary by url params query, form and accept headers */
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" });

            /* Can also call just without keys and it will cache urls based on base urls only. */
            //this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines);

            /* Enable this for all requests to be cached configured */
            //pipelines.AfterRequest.AddItemToStartOfPipeline(ConfigureCache);

            /* Enable cache using the DiskCacheStore, vary by url query, form and accept headers */
            //this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" }, new DiskCacheStore("c:/tmp/cache"));

            /* Enable cache using Redis , using the same key variations */
            //this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" }, new RedisCacheStore("localhost"));

            /* Enable cache using Couchbase , using the same key variations */
            //Couchbase requires the nuget package: https://www.nuget.org/packages/CouchbaseNetClient/

            /*                
                var cluster = new Cluster(new ClientConfiguration
	            {
	                Servers = new List<Uri> { new Uri("http://127.0.0.1") }
	            });
	
	            var authenticator = new PasswordAuthenticator("user1", "password");
	            cluster.Authenticate(authenticator);
             */

            //this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" }, new CouchbaseCacheStore(cluster, "myBucket"));
        }

        public void ConfigureCache(NancyContext context)
        {
            if (context.Response.StatusCode == HttpStatusCode.OK &&
                (context.Request.Method == "GET" ||
                context.Request.Method == "HEAD"))
            {
                context.Response = context.Response.AsCacheable(System.DateTime.UtcNow.AddSeconds(30));
            }
        }
    }
}
