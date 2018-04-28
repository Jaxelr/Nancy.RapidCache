using Nancy;
using Nancy.RapidCache.Extensions;
using System;
using System.Globalization;

namespace Asp.Net.Example
{
    public class Module : NancyModule
    {
        private int _cachedTime = 1000;

        public Module()
        {
            Get("/", _ =>
            {
                return
                    View["TestView.html", new { Hello = DateTime.Now.ToString(CultureInfo.InvariantCulture) }]
                    .AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime));
            });

            Get("/CachedResponse", _ =>
            {
                return Response
                    .AsText($"this is a cached response: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}")
                    .AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime));
            });

            Get("/CachedResponse/{hello}", dict =>
            {
                return Response.AsText($"Hello {dict.hello}, this is a cached response: {Guid.NewGuid()}")
                    .AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime));
            });

            Get("/faultyResponse", _ =>
            {
                return Response
                    .AsText($"This is a faulty response.")
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime));
            });
        }
    }
}
