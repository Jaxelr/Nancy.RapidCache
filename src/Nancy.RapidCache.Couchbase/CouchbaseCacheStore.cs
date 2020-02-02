using Couchbase.Core;
using Nancy.RapidCache.Projection;
using System;

namespace Nancy.RapidCache.CacheStore
{
    public class CouchbaseCacheStore : ICacheStore
    {
        private readonly ICluster cluster;
        private readonly string bucketname;

        public CouchbaseCacheStore(ICluster cluster, string bucketname)
        {
            this.cluster = cluster;
            this.bucketname = bucketname;
        }

        public CachedResponse Get(string key)
        {
            using (var bucket = cluster.OpenBucket(bucketname))
            {
                var result = bucket.Get<SerializableResponse>(key);

                if (result.Success)
                {
                    return new CachedResponse(result.Value);
                }
            }

            return null;
        }

        public void Remove(string key)
        {
            using (var bucket = cluster.OpenBucket(bucketname))
            {
                bucket.Remove(key);
            }
        }

        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            var span = absoluteExpiration - DateTime.UtcNow;

            using (var bucket = cluster.OpenBucket(bucketname))
            {
                if (context?.Response is Response)
                { 
                    var serialize = new SerializableResponse(context.Response, absoluteExpiration);
                    var result = bucket.Upsert(key, serialize, span);

                    if (!result.Success)
                    {
                        throw new Exception($"Could not complete operation of Upsert, using cluster configuration: {cluster.Configuration}");
                    }
                }
            }
        }
    }
}
