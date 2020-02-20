using System;
using Microsoft.Extensions.Caching.Memory;
using Nancy.RapidCache.Projection;

namespace Nancy.RapidCache.CacheStore
{
    public class IMemoryCacheStore : ICacheStore
    {
        private readonly IMemoryCache cache;
        private readonly long sizeLimit;
        private long size = 0;

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

        /// <summary>
        /// Tries to get the value from the MemoryCache using the key provided.
        /// If not found, returns null.
        /// If the key is null or empty, return null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CachedResponse Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            cache.TryGetValue(key, out SerializableResponse value);

            return (value == null) ? null : new CachedResponse(value);
        }

        /// <summary>
        /// Remove the object cache with the key provided.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => cache.Remove(key);

        /// <summary>
        /// Set the reponse of the cache as based on the response and the absolute expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="absoluteExpiration"></param>
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

        /// <summary>
        /// Verify the cache contains the key indicated.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool ContainsKey(string key) => cache.TryGetValue(key, out _);

        /// <summary>
        /// Once the record is evicted from the cache, subtract 1 from  the size
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        /// <param name="state"></param>
        private void Eviction(object key, object value, EvictionReason reason, object state) => size--;
    }
}
