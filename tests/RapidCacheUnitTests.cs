using Nancy.RapidCache.Tests.Mocks;
using Nancy.Testing;
using Xunit;

namespace Nancy.RapidCache.Tests
{
    public class RapidCacheUnitTests
    {
        private const string CACHED_RESPONSE_PATH = "/CachedResponse";

        [Fact]
        public void Cached_response_request()
        {
            //Arrange
            var browser = new Browser(new MockMemoryBootstrapper());

            //Act
            var response = browser.Get(CACHED_RESPONSE_PATH);
            System.Threading.Thread.Sleep(100);
            var response2 = browser.Get(CACHED_RESPONSE_PATH);

            //Assert
            Assert.Contains(response.Result.Body.AsString(), response2.Result.Body.AsString());
        }
    }
}
