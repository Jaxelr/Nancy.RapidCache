using Nancy.RapidCache.Projection;
using System;
using System.Collections.Concurrent;

namespace Nancy.RapidCache.CacheStore
{
    /// <summary>
    /// Stores responses serialized as JSON on a Concurrent Dictionary.
    /// Key is expected to be provided by the client.
    /// </summary>
    public class MemoryCacheStore : ICacheStore
    {
        private ConcurrentDictionary<string, SerializableResponse> _cache;
        private readonly int _maxSize = 0;

        public MemoryCacheStore()
        {
            _cache = new ConcurrentDictionary<string, SerializableResponse>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaxSize">Specifies the maximum size of items the cache can hold.</param>
        public MemoryCacheStore(int MaxSize)
        {
            _maxSize = MaxSize;
            _cache = new ConcurrentDictionary<string, SerializableResponse>();
        }

        /// <summary>
        /// Tries to get the value from the Concurrent Dictionary using the key provided.
        /// If not found, returns null.
        /// </summary>
        /// <param name="key">The unique key provided by the client</param>
        /// <returns></returns>
        public CachedResponse Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            _cache.TryGetValue(key, out SerializableResponse value);

            return (value == null) ? null : new CachedResponse(value);
        }

        /// <summary>
        /// Removes the value from the Concurrent Dictionary using the key provided.
        /// </summary>
        /// <param name="key">The unique key provided by the client</param>
        public void Remove(string key) => _cache.TryRemove(key, out SerializableResponse value);

        /// <summary>
        /// Sets the value from the Concurrent Dictionary using the key provided.
        /// Also includes an expiration date.
        /// </summary>
        /// <param name="key">The unique key provided by the client</param>
        /// <param name="context">The nancy context that contains the Response</param>
        /// <param name="absoluteExpiration">The expiration of the value on the dictionary</param>
        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_maxSize > 0 && _maxSize == _cache.Count && !_cache.ContainsKey(key))
            {
                return;
            }

            if (context.Response is Response response && absoluteExpiration > DateTime.UtcNow)
            {
                _cache[key] = new SerializableResponse(response, absoluteExpiration);
            }
        }
    }
}
