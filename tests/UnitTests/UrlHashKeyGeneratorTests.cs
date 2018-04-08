using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.Tests.Mocks;
using System.Collections.Generic;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class UrlHashKeyGeneratorTests
    {
        private const string ACCEPT = "Accept";
        private const string ACCEPT_VALUE = "text/plain";
        private const string METHOD = "GET";
        private const string PATH = "FakierPath";
        private readonly Dictionary<string, IEnumerable<string>> Acceptheader = new Dictionary<string, IEnumerable<string>>();

        public UrlHashKeyGeneratorTests()
        {
            Acceptheader.Add(ACCEPT, new[] { ACCEPT_VALUE });
        }

        [Fact]
        public void Keyed_by_url()
        {
            var keyGen = new UrlHashKeyGenerator();
            var request = new FakeRequest(
                method: METHOD,
                path: PATH,
                headers: Acceptheader);

            //Act
            string key = keyGen.Get(request);
            byte[] bytekey = System.Convert.FromBase64String(key);

            //Assert

            //Validating lengths from hash created.
            Assert.Equal(24, key.Length);
            Assert.Equal(16, bytekey.Length);
        }
    }
}
