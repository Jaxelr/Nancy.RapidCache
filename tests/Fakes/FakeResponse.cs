using System.IO;
using System.Text;

namespace Nancy.RapidCache.Tests.Fakes
{
    public class FakeResponse : Response
    {
        public FakeResponse()
        {
            StatusCode = HttpStatusCode.OK;
            ContentType = "text/plain";
            Contents = stream => new MemoryStream(Encoding.UTF8.GetBytes("Hello World"));
        }
    }
}
