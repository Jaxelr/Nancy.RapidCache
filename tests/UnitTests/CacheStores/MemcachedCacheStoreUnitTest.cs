using Enyim.Caching;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.RapidCache.Tests.Helpers;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class MemcachedCacheStoreUnitTest
    {
        private const string TEST_KEY_1 = "MemcachedRequest1";
        private const string TEST_KEY_2 = "MemcachedRequest2";
        private const string TEST_KEY_3 = "MemcachedRequest3";

        private MemcachedClient GetClient(MemcachedProtocol protocol = MemcachedProtocol.Binary, bool useBinaryFormatterTranscoder = false)
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddEnyimMemcached(options =>
            {
                options.AddServer("127.0.0.1", 11211);
            });

            services.AddLogging();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return  serviceProvider.GetService<IMemcachedClient>() as MemcachedClient;
        }

        //[Fact]
        //public void Memcached_cache_set_get()
        //{
        //    //Arrange
        //    var expirationDate = DateTime.UtcNow.AddMinutes(15);
        //    var cache = new MemcachedCacheStore(GetClient());
        //    var context = new NancyContext() { Response = new FakeResponse() { } };

        //    //Act
        //    cache.Set(TEST_KEY_1, context, expirationDate);
        //    var response = cache.Get(TEST_KEY_1);

        //    //Assert
        //    Assert.Equal(context.Response.ContentType, response.ContentType);
        //    Assert.Equal(context.Response.StatusCode, response.StatusCode);
        //    Assert.Equal(expirationDate, response.Expiration);
        //    Assert.Equal(context.Response.Contents.ConvertStream(), response.Contents.ConvertStream());
        //}

        [Fact]
        public void Memcached_cache_set_get_expired()
        {
            //Arrange
            var expiredDate = DateTime.UtcNow;
            var cache = new MemcachedCacheStore(GetClient());
            var context = new NancyContext() { Response = new FakeResponse() { } };

            //Act
            cache.Set(TEST_KEY_3, context, expiredDate);
            var response = cache.Get(TEST_KEY_3);

            //Assert
            Assert.Null(response);
            Assert.NotNull(context.Response);
        }

        [Fact]
        public void Memcached_cache_set_remove_get()
        {
            //Arrange
            var expirationDate = DateTime.Now.AddMinutes(15);
            var cache = new MemcachedCacheStore(GetClient());
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
