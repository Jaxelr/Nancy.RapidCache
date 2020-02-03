using System;
using Microsoft.Extensions.Caching.Memory;
using Nancy.RapidCache.Projection;

namespace Nancy.RapidCache.CacheStore
{
    public class IMemoryCacheStore : ICacheStore
    {
        private readonly IMemoryCache cache;

        public IMemoryCacheStore() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public IMemoryCacheStore(MemoryCacheOptions options) : this(new MemoryCache(options))
        {
        }

        public IMemoryCacheStore(MemoryCache cache)
        {
            this.cache = cache;
        }

        public CachedResponse Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            cache.TryGetValue(key, out SerializableResponse value);

            return (value == null) ? null : new CachedResponse(value);
        }

        public void Remove(string key) => cache.Remove(key);

        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            var span = absoluteExpiration - DateTime.UtcNow;

            if (context?.Response is Response response && span.TotalSeconds > 0)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(span);

                var serializable = new SerializableResponse(response, absoluteExpiration);
                cache.Set(key, serializable, options);
            }
        }
    }
}
