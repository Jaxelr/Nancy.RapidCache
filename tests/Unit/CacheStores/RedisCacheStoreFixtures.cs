using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.RapidCache.Tests.Helpers;
using StackExchange.Redis;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.Unit.CacheStores
{
    public class RedisCacheStoreFixtures
    {
        private const string LOCALHOST = "127.0.0.1:6379";

        [Theory]
        [InlineData(LOCALHOST, "RedisRequest1")]
        public void Redis_connect_using_configurations(string localhost, string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);

            var configOptions = ConfigurationOptions.Parse(localhost);
            var cache = new RedisCacheStore(configOptions);

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
        [InlineData(LOCALHOST, "RedisRequest2")]
        public void Redis_cache_set_get(string localhost, string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new RedisCacheStore(localhost);
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
        [InlineData(LOCALHOST, "RedisRequest3")]
        public void Redis_cache_set_get_empty(string localhost, string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new RedisCacheStore(localhost);
            
            //Act
            cache.Set(key, null, expirationDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
        }


        [Theory]
        [InlineData(LOCALHOST, "RedisRequest4")]
        public void Redis_cache_set_get_expired(string localhost, string key)
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new RedisCacheStore(localhost);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expiredDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Theory]
        [InlineData(LOCALHOST, "RedisRequest5")]
        public void Redis_cache_set_remove_get(string localhost, string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new RedisCacheStore(localhost);
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
        [InlineData(LOCALHOST, "RedisRequest6")]
        public void Redis_cache_set_empty_object(string localhost, string key)
        {
            //Arrange
            var expiredDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new RedisCacheStore(localhost);

            var context = new NancyContext() { Response = null };

            //Act
            cache.Set(key, context, expiredDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(context.Response);
            Assert.Null(response);
        }
    }
}
