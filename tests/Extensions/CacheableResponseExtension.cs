using Nancy.RapidCache.Projection;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Nancy.RapidCache.Tests.Extensions
{
    public static class CacheableResponseExtension
    {
        public static string GetContents(this CacheableResponse cacheableResponse)
        {
            string output;

            using (var memoryStream = new MemoryStream())
            {
                cacheableResponse.Contents(memoryStream);
                memoryStream.TryGetBuffer(out ArraySegment<byte> buffer);
                output = Encoding.UTF8.GetString(buffer.Where(a => a != 0).ToArray());
            }

            return output;
        }
    }
}
