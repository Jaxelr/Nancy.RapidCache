using Enyim.Caching;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Projection;
using System;

namespace Nancy.RapidCache.MemCached
{
    public class MemCachedCacheStore : ICacheStore
    {
        private readonly IMemcachedClient _cache;

        public MemCachedCacheStore(IMemcachedClient cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CachedResponse Get(string key)
        {
            var result = _cache.Get<SerializableResponse>(key);

            if (result is SerializableResponse)
            {
                return new CachedResponse(result);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => _cache.Remove(key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="absoluteExpiration"></param>
        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            int length = (absoluteExpiration - DateTime.UtcNow).Seconds;

            if (context.Response is Response && length > 0)
            {
                var serialize = new SerializableResponse(context.Response, absoluteExpiration);
                _cache.Set(key, serialize, length);
            }
        }
    }
}
