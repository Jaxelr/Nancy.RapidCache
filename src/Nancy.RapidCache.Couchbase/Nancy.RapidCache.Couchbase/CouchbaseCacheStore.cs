using Nancy.RapidCache.Projection;
using System;

namespace Nancy.RapidCache.CacheStore
{
    public class CouchbaseCacheStore : ICacheStore
    {
        public CachedResponse Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            throw new NotImplementedException();
        }
    }
}
