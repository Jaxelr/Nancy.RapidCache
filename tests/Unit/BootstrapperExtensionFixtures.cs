using Nancy.Bootstrapper;
using Nancy.RapidCache.Extensions;
using Nancy.RapidCache.Tests.Fakes;
using Xunit;

namespace Nancy.RapidCache.Tests.Unit
{
    [TestCaseOrderer("Nancy.RapidCache.Tests.AlphabeticalTestOrderer", "Nancy.RapidCache.Tests")]
    public class BootstrapperExtensionFixtures
    {
        [Fact]
        public void Disable_bootstrapper_with_cache_disable_key()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();

            //Act
            Assert.Throws<BootstrapperException>(() => bootstrapper.EnableCacheDisableKey(null));

            //Assert
            Assert.False(RapidCache.IsCacheEnabled());
            Assert.False(Defaults.DisableCache.Enabled);
        }

        [Fact]
        public void Disable_bootstrapper_with_cache_removal_key()
        {
            //Arrange
            var bootstrapper = new FakeDefaultBootstrapper();

            //Act
            Assert.Throws<BootstrapperException>(() => bootstrapper.EnableCacheDisableKey(null));

            //Assert
            Assert.False(RapidCache.IsCacheEnabled());
            Assert.False(Defaults.DisableCache.Enabled);
        }

        [Theory]
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

        [Theory]
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
