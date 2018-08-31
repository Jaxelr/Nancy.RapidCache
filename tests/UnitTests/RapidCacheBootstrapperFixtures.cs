using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.Testing;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class RapidCacheBootstrapperFixtures
    {
        private const string CACHED_RESPONSE_PATH = "/CachedResponse";

        [Fact]
        public void Cached_response_request()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();
            var browser = new Browser(bootstrapper);

            //Act
            var response = browser.Get(CACHED_RESPONSE_PATH);
            System.Threading.Thread.Sleep(100);
            var response2 = browser.Get(CACHED_RESPONSE_PATH);

            //Assert
            Assert.Contains(response.Result.Body.AsString(), response2.Result.Body.AsString());
        }

        [Fact]
        public void Enable_bootstrapper_with_keys_and_store()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();
            var routeResolver = new FakeRouteResolver();

            //Act
            RapidCache.Enable(bootstrapper, routeResolver, new FakePipelines(), new DefaultCacheKeyGenerator(), new MemoryCacheStore());

            //Assert
            Assert.True(RapidCache.IsCacheEnabled());
        }

        [Fact]
        public void Enable_bootstrapper_with_keys()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();
            var routeResolver = new FakeRouteResolver();

            //Act
            RapidCache.Enable(bootstrapper, routeResolver, new FakePipelines(), new DefaultCacheKeyGenerator());

            //Assert
            Assert.True(RapidCache.IsCacheEnabled());
        }


        [Fact]
        public void Enable_bootstrapper_with_keys_and_array_string()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();
            var routeResolver = new FakeRouteResolver();

            //Act
            RapidCache.Enable(bootstrapper, routeResolver, new FakePipelines(), new[] { "query", "form", "accept" });

            //Assert
            Assert.True(RapidCache.IsCacheEnabled());
        }
    }
}
