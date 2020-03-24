using Nancy.RapidCache.Extensions;
using System;
using System.Globalization;

namespace Nancy.RapidCache.SampleApplication.Net452
{
    public class Module : NancyModule
    {
        private readonly int cachedTime = 25;

        public Module()
        {
            Get("/", _ => View["TestView.html", new { Hello = DateTime.Now.ToString(CultureInfo.InvariantCulture) }].AsCacheable(DateTime.UtcNow.AddSeconds(cachedTime)));

            Get("/CachedResponse", _ => Response
                                        .AsText($"this is a cached response: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}")
                                        .AsCacheable(DateTime.UtcNow.AddSeconds(cachedTime)));

            Get("/faultyResponse", _ => new Response()
            {
                StatusCode = HttpStatusCode.InternalServerError
            }.AsCacheable(DateTime.UtcNow.AddSeconds(cachedTime)));

            Get("/faultyConditionalResponse", _ => new Response()
            {
                StatusCode = (string) Request.Query.fault.Value == "true" ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
            }.AsCacheable(DateTime.UtcNow.AddSeconds(cachedTime)));
        }
    }
}
