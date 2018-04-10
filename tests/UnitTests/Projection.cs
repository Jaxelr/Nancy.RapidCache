using Nancy.RapidCache.Extensions;
using Nancy.RapidCache.Projection;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class Projection
    {
        private DateTime expirationDate = DateTime.Now.AddMinutes(15);
        private string CACHE_KEY = Helper.CacheHeader;

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

        [Fact]
        public void Cacheable_negotiator_created()
        {
            //Arrange
            var context = new NancyContext() { Response = new FakeResponse() { } };
            var negotiator = new Negotiator(context);
            var headerKey = new KeyValuePair<string, string>(CACHE_KEY, expirationDate.ToString(CultureInfo.InvariantCulture));

            //Act
            var cacheableNegotiator = negotiator.AsCacheable(expirationDate);

            //Assert
            Assert.NotNull(cacheableNegotiator);
            Assert.True(cacheableNegotiator.NegotiationContext.Headers.ContainsKey(CACHE_KEY));
            Assert.True(cacheableNegotiator.NegotiationContext.Headers.Contains(headerKey));
        }
    }
}
