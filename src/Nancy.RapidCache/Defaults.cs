namespace Nancy.RapidCache
{
    public static class Defaults
    {
        public static (bool Enabled, string Key) RemoveCache = (false, "rapidCacheRemove");
        public static (bool Enabled, string Key) DisableCache = (false, "rapidCacheDisabled");
        public static readonly string CacheHeader = "nancy-rapidcache";
        public static readonly string CacheExpiry = "X-Nancy-RapidCache-Expiration";
    }
}
