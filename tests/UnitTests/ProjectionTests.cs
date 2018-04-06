using Nancy.RapidCache.Projection;
using Nancy.RapidCache.Tests.Fakes;
using System;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class ProjectionTests
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
        }
    }
}
