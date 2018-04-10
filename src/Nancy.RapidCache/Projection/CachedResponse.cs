using System;
using System.Globalization;
using System.IO;

namespace Nancy.RapidCache.Projection
{
    /// <summary>
    /// Cached Nancy Response
    /// </summary>
    public class CachedResponse : Response
    {
        public readonly string OldResponseOutput;

        public readonly DateTime Expiration;

        public CachedResponse(SerializableResponse response)
        {
            ContentType = response.ContentType;
            Headers = response.Headers;
            StatusCode = response.StatusCode;
            OldResponseOutput = response.Contents;
            Contents = GetContents(OldResponseOutput);
            Expiration = response.Expiration;

            Headers[Helper.CacheExpiry] = response.Expiration.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the content from the string sent to a Stream.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        protected static Action<Stream> GetContents(string contents)
         => stream =>
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.Write(contents);
            };
    }
}
