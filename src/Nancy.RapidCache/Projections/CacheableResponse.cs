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
            if (expiration.Kind != DateTimeKind.Utc)
                Expiration = expiration.ToUniversalTime();
            else
            {
                Expiration = expiration;
            }

            _response = response;
            ContentType = response.ContentType;
            Headers = response.Headers;
            StatusCode = response.StatusCode;
            Contents = _response.Contents;
        }
    }
}
