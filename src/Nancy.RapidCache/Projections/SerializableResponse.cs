using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nancy.RapidCache.Projection
{
    /// <summary>
    /// Persistable Nancy Response
    /// </summary>
    [Serializable]
    public class SerializableResponse
    {
        public string ContentType { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Contents { get; set; }
        public DateTime Expiration { get; set; }

        public SerializableResponse()
        {
        }

        public SerializableResponse(Response response, DateTime expiration)
        {
            Expiration = expiration;
            ContentType = response.ContentType;
            Headers = response.Headers;
            StatusCode = response.StatusCode;

            using (var memoryStream = new MemoryStream())
            {
                response.Contents(memoryStream);
                memoryStream.TryGetBuffer(out ArraySegment<byte> buffer);
                Contents = Encoding.UTF8.GetString(buffer.Where(a => a != 0).ToArray());
            }
        }
    }
}
