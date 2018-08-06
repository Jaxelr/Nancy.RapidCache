using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.RapidCache.Tests.Helpers;
using StackExchange.Redis;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class RedisCacheStoreUnitTest
    {
        private const string TEST_KEY_1 = "RedisRequest1";
        private const string TEST_KEY_2 = "RedisRequest2";
        private const string TEST_KEY_3 = "RedisRequest3";
        private const string TEST_KEY_4 = "RedisRequest4";
        private const string TEST_KEY_5 = "RedisRequest5";
        private const string LOCALHOST = "127.0.0.1:6379";

        [Fact]
        public void Redis_connect_using_configurations()
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);

            var configOptions = ConfigurationOptions.Parse(LOCALHOST);
            var cache = new RedisCacheStore(configOptions);

            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(TEST_KEY_4, context, expirationDate);
            var response = cache.Get(TEST_KEY_4);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        }

        [Fact]
        public void Redis_cache_set_get()
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new RedisCacheStore(LOCALHOST);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(TEST_KEY_1, context, expirationDate);
            var response = cache.Get(TEST_KEY_1);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        }

        [Fact]
        public void Redis_cache_set_get_expired()
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new RedisCacheStore(LOCALHOST);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(TEST_KEY_3, context, expiredDate);
            var response = cache.Get(TEST_KEY_3);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Fact]
        public void Redis_cache_set_remove_get()
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new RedisCacheStore(LOCALHOST);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(TEST_KEY_2, context, expirationDate);

            cache.Remove(TEST_KEY_2);

            var response = cache.Get(TEST_KEY_2);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }


        [Fact]
        public void Redis_cache_set_empty_object()
        {
            //Arrange
            var expiredDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new RedisCacheStore(LOCALHOST);
                        
            var context = new NancyContext() { Response = null };

            //Act
            cache.Set(TEST_KEY_5, context, expiredDate);
            var response = cache.Get(TEST_KEY_5);

            //Assert
            Assert.Null(context.Response);
            Assert.Null(response);
        }
    }
}

