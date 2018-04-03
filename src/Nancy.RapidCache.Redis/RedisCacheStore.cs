using Nancy.RapidCache.Projection;
using StackExchange.Redis;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nancy.RapidCache.CacheStore
{
    /// <summary>
    /// Implementation for usage with Redis Cache.
    /// </summary>
    public class RedisCacheStore : ICacheStore
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _cache;

        public RedisCacheStore(string configurations)
        {
            _redis = ConnectionMultiplexer.Connect(configurations);
            _cache = _redis.GetDatabase();
        }

        /// <summary>
        /// Removes the object from the cache by key.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) => _cache.KeyDelete(key);

        /// <summary>
        /// Gets the serializable object from redis.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CachedResponse Get(string key)
        {
            var result = (_cache.StringGet(key));

            if (result.HasValue)
            {
                var value = Deserialize<SerializableResponse>(result);
                return new CachedResponse(value);
            }

            return null;
        }

        /// <summary>
        ///  Sets the response of the context object as a serialized cached into the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="absoluteExpiration"></param>
        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            int length = (absoluteExpiration - DateTime.Now).Seconds;
            var span = new TimeSpan(0, 0, length);

            if (context.Response is CacheableResponse && length > 0)
            {
                var serialize = new SerializableResponse(context.Response, absoluteExpiration);
                _cache.StringSet(key, Serialize(serialize), expiry: span);
            }
        }

        /// <summary>
        /// Serialize the object into an array of bytes using the binary formatter
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static byte[] Serialize(object o)
        {
            byte[] objectDataAsStream = null;

            if (o != null)
            {
                var binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, o);
                    objectDataAsStream = memoryStream.ToArray();
                }
            }

            return objectDataAsStream;
        }

        /// <summary>
        /// Deserialize the array of bytes into a poco using the binary formatter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static T Deserialize<T>(byte[] stream)
        {
            var result = default(T);

            if (stream != null)
            {
                var binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream(stream))
                {
                    result = (T) binaryFormatter.Deserialize(memoryStream);
                }
            }

            return result;
        }
    }
}
