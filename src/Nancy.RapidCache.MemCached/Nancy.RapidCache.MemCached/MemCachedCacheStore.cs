using Enyim.Caching;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Projection;
using System;

namespace Nancy.RapidCache.CacheStore
{
    public class MemcachedCacheStore : ICacheStore
    {
        private readonly MemcachedClient _cache;

        public MemcachedCacheStore(MemcachedClient cache)
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

        public MemcachedCacheStore(Enyim.Caching.Configuration.MemcachedClientConfiguration config)
        {
            _cache = new MemcachedClient(config);
        }
#endif

        /// <summary>
        /// Remove the key from the cache
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => _cache.ExecuteRemove(key);

        /// <summary>
        /// Gets the key from the cache or returns null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CachedResponse Get(string key)
        {

            var result = _cache.ExecuteTryGet(key, out object tmp);

            if (result.Success)
            {
#if NET45
                return new CachedResponse((SerializableResponse)tmp);
#else
                //Clunky.
                var obj = Newtonsoft.Json.Linq.JObject.FromObject(tmp);
                return new CachedResponse(obj.ToObject<SerializableResponse>());
#endif
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

                 _cache.ExecuteStore(Enyim.Caching.Memcached.StoreMode.Set, key, context, absoluteExpiration);
            }
        }
    }
}
