namespace Nancy.RapidCache
{
    public static class Defaults
    {
#if NETSTANDARD2_0
        public static (bool Enabled, string Key) RemoveCache = (false, "rapidCacheRemove");
        public static (bool Enabled, string Key) DisableCache = (false, "rapidCacheDisabled");
#else
        public static string NoRequestQueryKey = "rapidCacheDisabled";
        public static bool NoRequestQueryEnabled = false;
        public static string RemoveCacheKey = "rapidCacheRemove";
        public static bool RemoveCacheEnabled = false;
#endif
        public static readonly string CacheHeader = "nancy-rapidcache";
        public static readonly string CacheExpiry = "X-Nancy-RapidCache-Expiration";
    }
}
