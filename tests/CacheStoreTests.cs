using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Mocks;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests
{
    public class CacheStoreTests
    {
        [Fact]
        public void Memory_cache_set_get()
        {
            //Arrange
            string testKey = "MockKeyRequest1";
            var expirationDate = DateTime.Now.AddSeconds(60);
            var cache = new MemoryCacheStore();
            var context = new NancyContext() { Response = new MockResponse() { } };

            //Act
            cache.Set(testKey, context, expirationDate);
            var response = cache.Get(testKey);

            //Assert
            //Missing Contents comparison.
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
        }

        [Fact]
        public void Memory_cache_set_remove_get()
        {
            //Arrange
            string testKey = "MockKeyRequest1";
            var expirationDate = DateTime.Now.AddSeconds(60);
            var cache = new MemoryCacheStore();
            var context = new NancyContext() { Response = new MockResponse() { } };

            //Act
            cache.Set(testKey, context, expirationDate);

            cache.Remove(testKey);

            var response = cache.Get(testKey);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }
    }
}
