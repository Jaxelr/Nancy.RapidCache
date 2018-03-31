namespace Nancy.RapidCache.CacheKey
{
    /// <summary>
    /// CacheKeyGenerator meant to be consumed by Nancy.RapidCache
    /// </summary>
    public interface ICacheKeyGenerator
    {
        string Get(Request request);
    }
}
