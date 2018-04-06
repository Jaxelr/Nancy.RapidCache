using System.IO;
using System.Text;

namespace Nancy.RapidCache.Tests.Mocks
{
    public class MockResponse : Response
    {
        public MockResponse()
        {
            StatusCode = HttpStatusCode.OK;
            ContentType = "text/plain";
            Contents = stream => new MemoryStream(Encoding.UTF8.GetBytes("Hello World"));
        }
    }
}
