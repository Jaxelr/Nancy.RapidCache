using System;
using Microsoft.Extensions.Caching.Memory;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.RapidCache.Tests.Helpers;
using Xunit;

namespace Nancy.RapidCache.Tests.Unit.CacheStores
{
    public class IMemoryCacheStoreFixtures
    {
        [Fact]
        public void IMemory_cache_empty_get()
        {
            //Arrange
            var cache = new IMemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(string.Empty, context, DateTime.UtcNow.AddMinutes(1));
            var response = cache.Get(string.Empty);

            //Assert
            Assert.Null(response);
        }

        [Theory]
        [InlineData("IMemoryRequest1")]
        public void IMemory_cache_set_get_with_options(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore(new MemoryCacheOptions());
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);
            var response = cache.Get(key);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        }

        [Theory]
        [InlineData("IMemoryRequest1")]
        public void IMemory_cache_set_get(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);
            var response = cache.Get(key);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        }


        [Theory]
        [InlineData("IMemoryRequest2")]
        public void IMemory_cache_set_get_empty(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore();

            //Act
            cache.Set(key, null, expirationDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
        }

        [Fact]
        public void IMemory_cache_set_get_empty_key()
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore();

            //Act
            cache.Set(null, null, expirationDate);
            var response = cache.Get(null);

            //Assert
            Assert.Null(response);
        }

        [Theory]
        [InlineData("IMemoryRequest3")]
        public void IMemory_cache_set_get_expired(string key)
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new IMemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expiredDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Theory]
        [InlineData("IMemoryRequest4")]
        public void IMemory_cache_set_remove_get(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);

            cache.Remove(key);

            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Theory]
        [InlineData("IMemoryRequest5", 0)]
        public void IMemory_cache_set_with_max_size(string key, int sizeLimit)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore(new MemoryCacheOptions() { SizeLimit = sizeLimit });
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);
            var response = cache.Get(key);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        }


        [Theory]
        [InlineData("IMemoryRequest6", "IMemoryRequest7", 1)]
        public void IMemory_cache_set_full_with_max_size(string key, string key2, int sizeLimit)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore(new MemoryCacheOptions() { SizeLimit = sizeLimit });
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);
            cache.Set(key2, context, expirationDate);

            var response = cache.Get(key);
            var response2 = cache.Get(key2);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
            Assert.Null(response2);
        }

        [Theory]
        [InlineData("IMemoryRequest8", "IMemoryRequest9", 1)]
        public void IMemory_cache_set_full_with_max_size_using_constructor(string key, string key2, int sizeLimit)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new IMemoryCacheStore(sizeLimit);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);
            cache.Set(key2, context, expirationDate);

            var response = cache.Get(key);
            var response2 = cache.Get(key2);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
            Assert.Null(response2);
        }
    }
}
