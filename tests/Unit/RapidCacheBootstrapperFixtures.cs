using Nancy.Bootstrapper;
using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Extensions;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.Testing;
using Xunit;

namespace Nancy.RapidCache.Tests.Unit
{
    [TestCaseOrderer("Nancy.RapidCache.Tests.PriorityOrderer", "Nancy.RapidCache.Tests")]
    public class RapidCacheBootstrapperFixtures
    {
        private const string CACHED_RESPONSE_PATH = "/CachedResponse";

        [Fact, Priority(2)]
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
            Assert.NotNull(response.Result.Headers["X-Nancy-RapidCache-Expiration"]);
        }

        [Fact, Priority(3)]
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

        [Fact, Priority(3)]
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

        [Fact, Priority(3)]
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

        [Fact, Priority(1)]
        public void Disabled_bootstrapper_with_cache_disable_key()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();

            //Act
            Assert.Throws<BootstrapperException>(() => bootstrapper.EnableCacheDisableKey(null));

            //Assert
            Assert.False(RapidCache.IsCacheEnabled());
            Assert.False(Defaults.DisableCache.Enabled);
        }

        [Fact, Priority(1)]
        public void Disabled_bootstrapper_with_cache_removal_key()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();

            //Act
            Assert.Throws<BootstrapperException>(() => bootstrapper.EnableCacheDisableKey(null));

            //Assert
            Assert.False(RapidCache.IsCacheEnabled());
            Assert.False(Defaults.DisableCache.Enabled);
        }

        [Theory, Priority(3)]
        [InlineData("CustomRemoval1Key")]
        public void Enable_bootstrapper_with_cache_removal_key(string mockKey)
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();
            var routeResolver = new FakeRouteResolver();

            //Act
            RapidCache.Enable(bootstrapper, routeResolver, new FakePipelines());
            bootstrapper.EnableCacheRemovalKey(mockKey);

            //Assert
            Assert.True(RapidCache.IsCacheEnabled());
            Assert.Equal(Defaults.RemoveCache.Key, mockKey);
            Assert.True(Defaults.RemoveCache.Enabled);
        }

        [Theory, Priority(3)]
        [InlineData("CustomDisable1Key")]
        public void Enable_bootstrapper_with_cache_disable_key(string mockKey)
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();
            var routeResolver = new FakeRouteResolver();

            //Act
            RapidCache.Enable(bootstrapper, routeResolver, new FakePipelines());
            bootstrapper.EnableCacheDisableKey(mockKey);

            //Assert
            Assert.True(RapidCache.IsCacheEnabled());
            Assert.Equal(Defaults.DisableCache.Key, mockKey);
            Assert.True(Defaults.DisableCache.Enabled);
        }
    }
}
