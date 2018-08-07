using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.Tests.Fakes;
using System.Collections.Generic;
using Xunit;

namespace Nancy.RapidCache.Tests.UnitTests
{
    public class KeyGenerators
    {
        private const string ACCEPT = "Accept";
        private const string ACCEPT_VALUE = "application/json";
        private const string METHOD = "GET";
        private const string PATH = "FakePath";
        private const string PROTOCOL = "http";
        private const string QUERY = "?FakeQuery=FakeValue";
        private const string FORM = "FakeForm";
        private const string FORM_VALUE = "FakeFormValue";
        private readonly Dictionary<string, IEnumerable<string>> Acceptheader = new Dictionary<string, IEnumerable<string>>();

        public KeyGenerators()
        {
            Acceptheader.Add(ACCEPT, new[] { ACCEPT_VALUE });
        }


        [Fact]
        public void Default_keyed_empty()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator();

            //Act
            string key = keyGen.Get(null);

            //Assert
            Assert.Empty(key);
        }

        [Fact]
        public void Default_keyed_by_url_only()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator();
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_url_only_empty_array()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_query()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.Contains(QUERY, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_header()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(ACCEPT_VALUE, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_header_and_query()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(ACCEPT_VALUE, key);
            Assert.Contains(QUERY, key);
            Assert.Contains(PATH, key);
        }


        [Fact]
        public void Default_keyed_by_header_and_query_and_disabled_query()
        {
            //Arrange
            const string DISABLED = "&rapidCacheDisabled";
            string query = string.Concat(QUERY, DISABLED);

            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: query);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(ACCEPT_VALUE, key);
            Assert.Contains(QUERY, key);
            Assert.Contains(PATH, key);
            Assert.DoesNotContain(DISABLED, key);
        }

        [Fact]
        public void Default_keyed_by_form()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(FORM) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            request.Form[FORM] = FORM_VALUE;

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.Contains(FORM, key);
            Assert.Contains(FORM_VALUE, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_form_header_query()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(FORM), nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: Acceptheader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            request.Form[FORM] = FORM_VALUE;

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(ACCEPT_VALUE, key);
            Assert.Contains(QUERY, key);
            Assert.Contains(FORM, key);
            Assert.Contains(FORM_VALUE, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void UrlHashKey_Keyed_by_url()
        {
            //Arrange
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
