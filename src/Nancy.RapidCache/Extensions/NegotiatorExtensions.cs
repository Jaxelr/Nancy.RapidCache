using System;
using System.Globalization;
using Nancy.Responses.Negotiation;

namespace Nancy.RapidCache.Extensions
{
    public static class NegotiatorExtensions
    {
        /// <summary>
        /// Extensions method used to mark this Negotiator as cacheable by Nancy.RapidCache
        /// </summary>
        /// <param name="negotiator"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public static Negotiator AsCacheable(this Negotiator negotiator, DateTime expiration)
            => negotiator.WithHeader("nancy-RapidCache", expiration.ToString(CultureInfo.InvariantCulture));
    }
}
