using System;
using Nancy.RapidCache.Projection;

namespace Nancy.RapidCache.Extensions
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// Extension method used to mark this response as cacheable by Nancy.RapidCache
        /// </summary>
        /// <param name="response"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public static Response AsCacheable(this Response response, DateTime expiration) => new CacheableResponse(response, expiration);
    }
}
