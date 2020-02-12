using System;
using Microsoft.Extensions.Caching.Memory;
using Nancy.RapidCache.Projection;

namespace Nancy.RapidCache.CacheStore
{
    public class IMemoryCacheStore : ICacheStore
    {
        private readonly IMemoryCache cache;
        private long sizeLimit;

        public IMemoryCacheStore() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public IMemoryCacheStore(MemoryCacheOptions options) : this(new MemoryCache(options))
        {
            sizeLimit = options.SizeLimit ?? 0;
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

        public void Set(string key, NancyContext context, DateTime absoluteExpiration) => Set(key, context, absoluteExpiration, 0);

        public void Set(string key, NancyContext context, DateTime absoluteExpiration, long size)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var span = absoluteExpiration - DateTime.UtcNow;

            if (context?.Response is Response response && span.TotalSeconds > 0)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(span)
                    .SetSize(size);

                var serializable = new SerializableResponse(response, absoluteExpiration);
                cache.Set(key, serializable, options);
            }
        }
    }
}
