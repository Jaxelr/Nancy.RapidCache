using System;
using System.Security.Cryptography;
using System.Text;

namespace Nancy.RapidCache.CacheKey
{
    public class UrlHashKeyGenerator : ICacheKeyGenerator
    {
        public string Get(Request request)
        {
            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(request.Url.ToString()));
            return Convert.ToBase64String(hash);
        }
    }
}
