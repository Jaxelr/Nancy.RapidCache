using Nancy.RapidCache.Extensions;
using System;
using System.Globalization;

namespace Nancy.RapidCache.SampleApplication.Net452
{
    public class Module : NancyModule
    {
        private readonly int _cachedTime = 25;

        public Module()
        {
            Get["/"] = _ =>
            {
                return View["TestView.html", new { Hello = DateTime.Now.ToString(CultureInfo.InvariantCulture) }].AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime));
            };

            Get["/CachedResponse"] = _ =>
            {
                return Response.AsText(@"
                this is a cached response: " + DateTime.Now.ToString(CultureInfo.InvariantCulture) + @"
                ").AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime));
            };

            Get["/faultyResponse"] = _ =>
            {
                return new Response() { StatusCode = HttpStatusCode.InternalServerError }.AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime);
            };

            Get["/faultyConditionalResponse"] = _ =>
            {
                return new Response() { StatusCode = (string) Request.Query.fault.Value == "true" ? HttpStatusCode.InternalServerError : HttpStatusCode.OK }.AsCacheable(DateTime.UtcNow.AddSeconds(_cachedTime));
            };
        }
    }
}
