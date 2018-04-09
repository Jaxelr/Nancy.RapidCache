using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Nancy.RapidCache.Tests.Fakes
{
    public class FakeResponse : Response
    {
        public FakeResponse()
        {
            StatusCode = HttpStatusCode.OK;
            ContentType = "text/plain";
            Contents = stream => new MemoryStream(Encoding.UTF8.GetBytes($"Hello World, my custom id is: {Guid.NewGuid()}"));
        }

        public string GetContents()
        {
            string output;

            using (var memoryStream = new MemoryStream())
            {
                Contents(memoryStream);
                memoryStream.TryGetBuffer(out ArraySegment<byte> buffer);
                output = Encoding.UTF8.GetString(buffer.Where(a => a != 0).ToArray());
            }

            return output;
        }
    }
}
