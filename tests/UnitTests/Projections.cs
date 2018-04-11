using Nancy.RapidCache.Projection;
using Nancy.RapidCache.Tests.Extensions;
using Nancy.RapidCache.Tests.Fakes;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class Projections
    {
        private DateTime expirationDate = DateTime.Now.AddMinutes(15);

        [Fact]
        public void Cacheable_response_created()
        {
            //Arrange
            var fakeResponse = new FakeResponse();

            //Act
            var cacheableResponse = new CacheableResponse(fakeResponse, expirationDate);

            //Assert
            Assert.NotNull(cacheableResponse);
            Assert.Equal(fakeResponse.ContentType, cacheableResponse.ContentType);
            Assert.Equal(fakeResponse.Headers, cacheableResponse.Headers);
            Assert.Equal(fakeResponse.StatusCode, cacheableResponse.StatusCode);
            Assert.Equal(fakeResponse.GetContents(), cacheableResponse.GetContents());
        }

        [Fact]
        public void Serializable_response_created()
        {
            //Arrange
            var fakeResponse = new FakeResponse();

            //Act
            var serializableResponse = new SerializableResponse(fakeResponse, expirationDate);

            //Assert
            Assert.NotNull(serializableResponse);
            Assert.Equal(fakeResponse.ContentType, serializableResponse.ContentType);
            Assert.Equal(fakeResponse.Headers, serializableResponse.Headers);
            Assert.Equal(fakeResponse.StatusCode, serializableResponse.StatusCode);
            Assert.Equal(fakeResponse.GetContents(), serializableResponse.Contents);
        }

        [Fact]
        public void CachedResponse_response_created()
        {
            //Arrange
            var fakeResponse = new FakeResponse();

            //Act
            var cachedResponse = new CachedResponse(new SerializableResponse(fakeResponse, expirationDate));

            //Assert
            Assert.NotNull(cachedResponse);
            Assert.Equal(fakeResponse.ContentType, cachedResponse.ContentType);
            Assert.Equal(fakeResponse.Headers, cachedResponse.Headers);
            Assert.Equal(fakeResponse.StatusCode, cachedResponse.StatusCode);
            Assert.Equal(fakeResponse.GetContents(), cachedResponse.GetContentAsString());
        }
    }
}
