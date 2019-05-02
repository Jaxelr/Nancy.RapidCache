using System;
using System.Security.Cryptography;
using System.Text;

namespace Nancy.RapidCache.CacheKey
{
    public class UrlHashKeyGenerator : ICacheKeyGenerator
    {
        /// <summary>
        /// Take the Url of the request and create an md5 hash to return as a key.
        /// </summary>
        /// <param name="request">Nancy Request object that represents the http request</param>
        /// <returns></returns>
        public string Get(Request request)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(request.Url.ToString()));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
