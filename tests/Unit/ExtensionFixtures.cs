using Nancy.RapidCache.Extensions;
using Nancy.RapidCache.Tests.Fakes;
using Nancy.RapidCache.Tests.Helpers;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using static Nancy.RapidCache.Defaults;

namespace Nancy.RapidCache.Tests.Unit
{
    public class ExtensionFixtures
    {
        private DateTime expirationDate = DateTime.Now.AddMinutes(15);
        private readonly string cacheKey = CacheHeader;

        [Fact]
        public void Cacheable_negotiator_created()
        {
            //Arrange
            var context = new NancyContext() { Response = new FakeResponse() { } };
            var negotiator = new Negotiator(context);
            var headerKey = new KeyValuePair<string, string>(cacheKey, expirationDate.ToString(CultureInfo.InvariantCulture));

            //Act
            var cacheableNegotiator = negotiator.AsCacheable(expirationDate);

            //Assert
            Assert.NotNull(cacheableNegotiator);
            Assert.True(cacheableNegotiator.NegotiationContext.Headers.ContainsKey(cacheKey));
            Assert.True(cacheableNegotiator.NegotiationContext.Headers.Contains(headerKey));
        }

        [Fact]
        public void Cacheable_response_create()
        {
            //Arrange
            var fakeResponse = new FakeResponse();

            //Act
            var cacheableResponse = fakeResponse.AsCacheable(expirationDate);

            //Assert
            Assert.NotNull(cacheableResponse);
            Assert.Equal(fakeResponse.ContentType, cacheableResponse.ContentType);
            Assert.Equal(fakeResponse.StatusCode, cacheableResponse.StatusCode);
            Assert.Equal(fakeResponse.GetContents(), cacheableResponse.Contents.ConvertStream());
        }
    }
}
