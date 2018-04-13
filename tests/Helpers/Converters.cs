using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Nancy.RapidCache.Tests.Helpers
{
    public static class Converters
    {
        public static string ConvertStream(this Action<Stream> actionStream)
        {
            string output;

            using (var stream = new MemoryStream())
            {
                actionStream(stream);
                stream.TryGetBuffer(out ArraySegment<byte> buffer);
                output = Encoding.UTF8.GetString(buffer.Where(a => a != 0).ToArray());
            }

            return output;
        }
    }
}
