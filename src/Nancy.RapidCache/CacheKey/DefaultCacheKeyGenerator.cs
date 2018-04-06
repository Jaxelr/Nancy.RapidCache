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
        private IEnumerable<string> _varyParams;

        /// <summary>
        /// Defaults usage to url only
        /// </summary>
        public DefaultCacheKeyGenerator()
        {
        }

        public DefaultCacheKeyGenerator(string[] varyParams)
        {
            _varyParams = varyParams.Select(x => x.ToLowerInvariant());
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

            if (_varyParams is IEnumerable<string>)
            {
                if (request.Query is DynamicDictionary &&
                    _varyParams.Contains(nameof(request.Query).ToLowerInvariant()))
                {
                    var dynDict = (request.Query as DynamicDictionary);
                    foreach (string key in dynDict.Keys)
                    {
                        query[key] = (string) dynDict[key];
                    }
                }

                if (request.Form is DynamicDictionary &&
                    _varyParams.Contains(nameof(request.Form).ToLowerInvariant()))
                {
                    var dynDict = (request.Form as DynamicDictionary);
                    foreach (string key in dynDict.Keys)
                    {
                        query[key] = (string) dynDict[key];
                    }
                }

                if (request.Headers.Accept?.Count() > 0 &&
                    _varyParams.Contains(nameof(request.Headers.Accept).ToLowerInvariant()))
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
