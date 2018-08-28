﻿using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.RapidCache.Tests.Helpers;
using System;
using System.IO;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests.CacheStores
{
    public class DiskCacheStoreFixtures
    {
        private readonly string Path = @"c:\temp\";
        private TimeSpan TimeSpan => new TimeSpan(0, 0, 10);

        [Theory]
        [InlineData(@".\\invalid\\path")]
        public void Disk_cache_invalid_path(string invalidPath) => Assert.Throws<ArgumentException>(() => new DiskCacheStore(invalidPath));

        [Theory]
        [InlineData(@".\\invalid\\path")]
        public void Disk_cache_invalid_path_with_timespan(string invalidPath) => Assert.Throws<ArgumentException>(() => new DiskCacheStore(invalidPath, new TimeSpan()));

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
            Assert.True(Directory.Exists(Path));
        }

        [Fact]
        public void Disk_cache_empty_get_not_existing_custom_path()
        {
            //Arrange
            string tempPath = System.IO.Path.Combine(Path , "temp1");
            var cache = new DiskCacheStore(tempPath);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(string.Empty, context, DateTime.UtcNow.AddMinutes(1));
            var response = cache.Get(string.Empty);

            //Assert
            Assert.Null(response);
            Assert.True(Directory.Exists(Path));

            Directory.Delete(tempPath);
        }

        [Fact]
        public void Disk_cache_empty_get_with_timespan()
        {
            //Arrange
            var cache = new DiskCacheStore(Path, TimeSpan);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(string.Empty, context, DateTime.UtcNow.AddMinutes(1));
            var response = cache.Get(string.Empty);

            //Assert
            Assert.Null(response);
            Assert.True(Directory.Exists(Path));
        }

        [Fact]
        public void Disk_cache_empty_get_with_timespan_and_not_existing_custom_path()
        {
            //Arrange
            string tempPath = System.IO.Path.Combine(Path , "temp2");
            var cache = new DiskCacheStore(tempPath, TimeSpan);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(string.Empty, context, DateTime.UtcNow.AddMinutes(1));
            var response = cache.Get(string.Empty);

            //Assert
            Assert.Null(response);
            Assert.True(Directory.Exists(Path));

            Directory.Delete(tempPath);
        }

        [Theory]
        [InlineData("FileRequest1")]
        public void Disk_cache_set_get(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);
            var response = cache.Get(key);
            cache.Remove(key);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        }

        [Theory]
        [InlineData("FileRequest2")]
        public void Disk_cache_set_set_get(string key)
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);
            cache.Set(key, context, expirationDate);
            var response = cache.Get(key);
            cache.Remove(key);

            //Assert
            Assert.Equal(context.Response.ContentType, response.ContentType);
            Assert.Equal(context.Response.StatusCode, response.StatusCode);
            Assert.Equal(expirationDate, response.Expiration);
            Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        }

        [Theory]
        [InlineData("FileRequest3")]
        public void Disk_cache_set_get_expired(string key)
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expiredDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }


        [Theory]
        [InlineData("FileRequest4")]
        public void Disk_cache_set_then_get_expired(string key)
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expiredDate);
            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Theory]
        [InlineData("FileRequest5")]
        public void Disk_cache_set_remove_get(string key)
        {
            //Arrange
            var expirationDate = DateTime.Now.AddMinutes(15);
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(key, context, expirationDate);

            cache.Remove(key);

            var response = cache.Get(key);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }
    }
}
