using System;
using Microsoft.Extensions.Caching.Memory;
using Nancy.RapidCache.Projection;

namespace Nancy.RapidCache.CacheStore
{
    public class IMemoryCacheStore : ICacheStore
    {
        private readonly IMemoryCache cache;
        private readonly long sizeLimit;
        private long size;

        public IMemoryCacheStore() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public IMemoryCacheStore(MemoryCacheOptions options) : this(new MemoryCache(options))
        {
            sizeLimit = options.SizeLimit ?? 0;
        }

        public IMemoryCacheStore(int maxSize) : this(new MemoryCacheOptions() { SizeLimit = maxSize })
        {
        }

        public IMemoryCacheStore(MemoryCache cache)
        {
            this.cache = cache;
            size = 0;
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
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (sizeLimit > 0 && sizeLimit == size && !ContainsKey(key))
            {
                return;
            }

            var span = absoluteExpiration - DateTime.UtcNow;

            if (context?.Response is Response response && span.TotalSeconds > 0)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(span)
                    .RegisterPostEvictionCallback(callback: Eviction, state: this)
                    .SetSize(size);

                var serializable = new SerializableResponse(response, absoluteExpiration);
                cache.Set(key, serializable, options);
                size++;
            }
        }

        private bool ContainsKey(string key) => cache.TryGetValue(key, out _);

        private void Eviction(object key, object value, EvictionReason reason, object state) => size--;
    }
}
