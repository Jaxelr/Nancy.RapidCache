using System;

namespace Nancy.RapidCache.Projection
{
    /// <summary>
    /// Cacheable Nancy Response
    /// </summary>
    public class CacheableResponse : Response
    {
        private readonly Response _response;

        public readonly DateTime Expiration;

        public CacheableResponse(Response response, DateTime expiration)
        {
            _response = response;
            ContentType = response.ContentType;
            Headers = response.Headers;
            StatusCode = response.StatusCode;
            Expiration = expiration.ToUniversalTime();
            Contents = _response.Contents;
        }
    }
}
