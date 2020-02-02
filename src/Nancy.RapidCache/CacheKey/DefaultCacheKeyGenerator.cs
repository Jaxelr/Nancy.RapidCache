using System;
using System.Collections.Generic;
using System.Linq;

namespace Nancy.RapidCache.CacheKey
{
    /// <summary>
    /// Default cache key use by the Container to obtain keys.
    /// Plausible values are query, form, accept
    /// By default the host, scheme, path and ports of an url are used as unique key.
    /// </summary>
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private readonly IEnumerable<string> varyParams;

        /// <summary>
        /// Defaults usage to url only
        /// </summary>
        public DefaultCacheKeyGenerator()
        {
        }

        public DefaultCacheKeyGenerator(string[] varyParams)
        {
            this.varyParams = varyParams.Select(x => x.ToLowerInvariant());
        }

        /// <summary>
        /// Generates a cache key from the supplied Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string Get(Request request)
        {
            if (request == null || request.Url == null)
                return string.Empty;

            var query = new Dictionary<string, string>();

            if (varyParams is IEnumerable<string>)
            {
                if (request.Query is DynamicDictionary dynQuery &&
                    varyParams.Contains(nameof(request.Query), StringComparer.InvariantCultureIgnoreCase))
                {
                    foreach (string key in dynQuery.Keys)
                    {
#if NETSTANDARD2_0
                        if (key.Equals(Defaults.DisableCache.Key, StringComparison.OrdinalIgnoreCase) ||
                            key.Equals(Defaults.RemoveCache.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
#else
                        if (key.Equals(Defaults.NoRequestQueryKey, StringComparison.OrdinalIgnoreCase) ||
                            key.Equals(Defaults.RemoveCacheKey, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
#endif

                        query[key] = (string) dynQuery[key];
                    }
                }

                if (request.Form is DynamicDictionary dynForm &&
                    varyParams.Contains(nameof(request.Form), StringComparer.InvariantCultureIgnoreCase))
                {
                    foreach (string key in dynForm.Keys)
                    {
                        query[key] = (string) dynForm[key];
                    }
                }

                if (request.Headers.Accept?.Count() > 0 &&
                    varyParams.Contains(nameof(request.Headers.Accept), StringComparer.InvariantCultureIgnoreCase))
                {
                    string acceptHeaders = string.Join(",", request.Headers.Accept.Select(x => x.Item1));
                    query.Add(nameof(request.Headers.Accept), acceptHeaders);
                }
            }

            var url = new Url
            {
                BasePath = request.Url.BasePath,
                HostName = request.Url.HostName,
                Path = request.Url.Path,
                Port = request.Url.Port,
                Query = string.Concat((query.Count > 0 ? "?" : string.Empty),
                        string.Join("&", query.Select(a => string.Join("=", a.Key, a.Value)))),
                Scheme = request.Url.Scheme
            };

            return url.ToString();
        }
    }
}
