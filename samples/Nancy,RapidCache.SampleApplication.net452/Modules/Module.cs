using Nancy;
using Nancy.RapidCache.Extensions;
using System;
using System.Globalization;

namespace Nancy.RapidCache.SampleApplication.Net452
{
    public class Module : NancyModule
    {
        public Module()
        {
            Get["/"] = _ =>
            {
                return View["TestView.html", new { Hello = DateTime.Now.ToString(CultureInfo.InvariantCulture) }].AsCacheable(DateTime.UtcNow.AddSeconds(25));
            };

            Get["/CachedResponse"] = _ =>
            {
                return Response.AsText(@"
                this is a cached response: " + DateTime.Now.ToString(CultureInfo.InvariantCulture) + @"
                ").AsCacheable(DateTime.UtcNow.AddSeconds(25));
            };

            Get["/faultyResponse"] = _ =>
            {
                return new Response() { StatusCode = HttpStatusCode.InternalServerError }.AsCacheable(DateTime.UtcNow.AddSeconds(30));
            };

            Get["/faultyConditionalResponse"] = _ =>
            {
                return new Response() { StatusCode = (string) Request.Query.fault.Value == "true" ? HttpStatusCode.InternalServerError : HttpStatusCode.OK }.AsCacheable(DateTime.UtcNow.AddSeconds(1));
            };
        }
    }
}
