using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests.CacheStores
{
    public class DiskCacheStoreFixtures
    {
        private readonly string Path = @"c:\temp\";

        [Fact]
        public void Disk_cache_empty_get()
        {
            //Arrange
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(string.Empty, context, DateTime.UtcNow.AddMinutes(1));
            var response = cache.Get(string.Empty);

            //Assert
            Assert.Null(response);
        }
    }
}
