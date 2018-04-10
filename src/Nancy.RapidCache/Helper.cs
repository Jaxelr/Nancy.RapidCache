namespace Nancy.RapidCache
{
    public static class Helper
    {
        public static readonly string NoRequestCacheKey = "_rapidCacheDisabled";
        public static readonly string CacheHeader = "nancy-rapidcache";
        public static readonly string CacheExpiry = "X-Nancy-RapidCache-Expiration";
    }
}
