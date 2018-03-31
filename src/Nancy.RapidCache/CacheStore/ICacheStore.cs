using System;
using Nancy.RapidCache.Projection;

namespace Nancy.RapidCache.CacheStore
{
    /// <summary>
    /// CacheStore meant to be consumed by Nancy.RapidCache
    /// </summary>
    public interface ICacheStore
    {
        CachedResponse Get(string key);

        void Set(string key, NancyContext context, DateTime absoluteExpiration);

        void Remove(string key);
    }
}
