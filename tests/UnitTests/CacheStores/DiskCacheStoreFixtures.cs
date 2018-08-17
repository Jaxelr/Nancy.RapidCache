using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Helpers;
using Nancy.RapidCache.Tests.Fakes;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests.CacheStores
{
    public class DiskCacheStoreFixtures
    {
        private readonly string Path = @"c:\temp\";
        private const string TEST_KEY_1 = "FileRequest1";
        private const string TEST_KEY_2 = "FileRequest2";
        private const string TEST_KEY_3 = "FileRequest3";

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


        [Fact]
        public void Disk_cache_set_get()
        {
            //Arrange
            var expirationDate = DateTime.UtcNow.AddMinutes(15);
            var cache = new DiskCacheStore(Path);
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
        public void Disk_cache_set_get_expired()
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(TEST_KEY_3, context, expiredDate);
            var response = cache.Get(TEST_KEY_3);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Fact]
        public void Disk_cache_set_remove_get()
        {
            //Arrange
            var expirationDate = DateTime.Now.AddMinutes(15);
            var cache = new DiskCacheStore(Path);
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(TEST_KEY_2, context, expirationDate);

            cache.Remove(TEST_KEY_2);

            var response = cache.Get(TEST_KEY_2);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }
    }
}
