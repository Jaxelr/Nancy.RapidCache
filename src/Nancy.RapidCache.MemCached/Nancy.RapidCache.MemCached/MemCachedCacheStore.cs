using Enyim.Caching;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Projection;
using System;

namespace Nancy.RapidCache.CacheStore
{
    public class MemcachedCacheStore : ICacheStore
    {
        private readonly IMemcachedClient _cache;

        public MemcachedCacheStore(IMemcachedClient cache)
        {
            _cache = cache;
        }

#if NET45
        public MemcachedCacheStore(string host, int port, Enyim.Caching.Memcached.MemcachedProtocol protocol = Enyim.Caching.Memcached.MemcachedProtocol.Text)
        {
            var config = new Enyim.Caching.Configuration.MemcachedClientConfiguration();

            config.AddServer(host, port);

            config.Protocol = protocol;

            _cache = new MemcachedClient(config);
        }
#endif

        /// <summary>
        /// Remove the key from the cache
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => _cache.Remove(key);

        /// <summary>
        /// Gets the key from the cache or returns null
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
        /// Update the key if the time hasnt expired.
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

                bool done = _cache.Store(Enyim.Caching.Memcached.StoreMode.Set, key, context);

                if (done)
                {
                }

            }
        }
    }
}
