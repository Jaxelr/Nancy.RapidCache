﻿using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.RapidCache.Tests.Helpers;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.Unit.CacheStores
{
    public class MemoryCacheStoreFixtures
    {
        [Fact]
        public void Memory_cache_empty_get()
        {
            //Arrange
            var cache = new MemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() };

            //Act
            cache.Set(string.Empty, context, DateTime.UtcNow.AddMinutes(1));
            var response = cache.Get(string.Empty);

            //Assert
            Assert.Null(response);
        }

        [Theory]
        [InlineData("MemoryRequest1")]
        public void Memory_cache_set_get(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new MemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() };

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
        [InlineData("MemoryRequest2")]
        public void Memory_cache_set_get_empty(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new MemoryCacheStore();
            
            //Act
            cache.Set(key, null, expirationDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
        }

        [Fact]
        public void Memory_cache_set_get_empty_key()
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new MemoryCacheStore();

            //Act
            cache.Set(null, null, expirationDate);
            var response = cache.Get(null);

            //Assert
            Assert.Null(response);
        }

        [Theory]
        [InlineData("MemoryRequest3")]
        public void Memory_cache_set_get_expired(string key)
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new MemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() };

            //Act
            cache.Set(key, context, expiredDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Theory]
        [InlineData("MemoryRequest4")]
        public void Memory_cache_set_remove_get(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new MemoryCacheStore();
            var context = new NancyContext() { Response = new FakeResponse() };

            //Act
            cache.Set(key, context, expirationDate);

            cache.Remove(key);

            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Theory]
        [InlineData("MemoryRequest5")]
        public void Memory_cache_set_with_max_size(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new MemoryCacheStore(1);
            var context = new NancyContext() { Response = new FakeResponse() };

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
        [InlineData("MemoryRequest6", "MemoryRequest7")]
        public void Memory_cache_set_full_with_max_size(string key, string key2)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new MemoryCacheStore(1);
            var context = new NancyContext() { Response = new FakeResponse() };

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
        [InlineData("MemoryRequest8", "MemoryRequest9")]
        public void Memory_cache_set_full_with_max_size_expired(string key, string key2)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddSeconds(1);
            var cache = new MemoryCacheStore(1);
            var context = new NancyContext() { Response = new FakeResponse() };

            //Act
            cache.Set(key, context, expirationDate);
            System.Threading.Thread.Sleep(1000);
            expirationDate = DateTime.UtcNow.AddSeconds(10);
            cache.Set(key2, context, expirationDate);

            var response = cache.Get(key);
            var response2 = cache.Get(key2);

            //Assert
            Assert.Equal(context.Response.ContentType, response2.ContentType);
            Assert.Equal(context.Response.StatusCode, response2.StatusCode);
            Assert.Equal(expirationDate, response2.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response2.Contents.ConvertStream());
        }


        [Theory]
        [InlineData("MemoryRequest10")]
        public void Memory_cache_set_full_with_max_size_same_key(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddSeconds(1);
            var cache = new MemoryCacheStore(1);
            var context = new NancyContext() { Response = new FakeResponse() };

            //Act
            cache.Set(key, context, expirationDate);
            cache.Set(key, context, expirationDate);

            var response = cache.Get(key);
            var response2 = cache.Get(key);

            //Assert
            Assert.Equal(context.Response.ContentType, response2.ContentType);
            Assert.Equal(context.Response.StatusCode, response2.StatusCode);
            Assert.Equal(expirationDate, response2.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response2.Contents.ConvertStream());
        }
    }
}
