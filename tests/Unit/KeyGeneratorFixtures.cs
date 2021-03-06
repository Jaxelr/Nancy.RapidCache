﻿using Nancy.RapidCache.CacheKey;
using Nancy.RapidCache.Tests.Fakes;
using System.Collections.Generic;
using Xunit;

namespace Nancy.RapidCache.Tests.Unit
{
    public class KeyGeneratorFixtures
    {
        private const string ACCEPT = "Accept";
        private const string ACCEPT_VALUE = "application/json";
        private const string METHOD = "GET";
        private const string PATH = "FakePath";
        private const string PROTOCOL = "http";
        private const string QUERY = "?FakeQuery=FakeValue";
        private const string FORM = "FakeForm";
        private const string FORM_VALUE = "FakeFormValue";
        private const string DISABLED = "&RapidCacheDisabled=true";
        private const string REMOVE = "&RapidCacheRemove=true";
        private readonly Dictionary<string, IEnumerable<string>> acceptHeader = new Dictionary<string, IEnumerable<string>>();

        public KeyGeneratorFixtures()
        {
            acceptHeader.Add(ACCEPT, new[] { ACCEPT_VALUE });
        }

        [Fact]
        public void Default_keyed_null()
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
                    headers: acceptHeader,
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
        public void Default_keyed_by_url_only_with_disable_key()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator();
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: string.Concat(QUERY, DISABLED));

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_url_only_with_disable_key_with_query()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: string.Concat(QUERY, DISABLED));

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(QUERY, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_url_only_with_removal_key()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator();
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: string.Concat(QUERY, REMOVE));

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.Contains(PATH, key);
        }


        [Fact]
        public void Default_keyed_by_url_only_with_removal_key_with_query()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: string.Concat(QUERY, REMOVE));

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(QUERY, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_url_only_with_disable_and_remove_key_with_query()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: string.Concat(QUERY, DISABLED,  REMOVE));

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(QUERY, key);
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
                    headers: acceptHeader,
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
                    headers: acceptHeader,
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
        public void Default_keyed_by_query_without_value()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.Contains(PATH, key);
        }


        [Fact]
        public void Default_keyed_by_query_without_declaration()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
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
        public void Default_keyed_by_query_with_another_param()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
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
                    headers: acceptHeader,
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
        public void Default_keyed_by_header_without_value()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: null,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(ACCEPT_VALUE, key);
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
                    headers: acceptHeader,
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
            string disabled = string.Concat("&", Defaults.RemoveCache.Key);
            string query = string.Concat(QUERY, disabled);

            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
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
            Assert.DoesNotContain(disabled, key);
        }

        [Fact]
        public void Default_keyed_by_form()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(FORM) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
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
        public void Default_keyed_by_form_without_value()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(FORM) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.DoesNotContain(FORM, key);
            Assert.DoesNotContain(FORM_VALUE, key);
            Assert.Contains(PATH, key);
        }


        [Fact]
        public void Default_keyed_by_form_without_declaration()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            request.Form[FORM] = FORM_VALUE;

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.DoesNotContain(ACCEPT, key);
            Assert.DoesNotContain(QUERY, key);
            Assert.DoesNotContain(FORM, key);
            Assert.DoesNotContain(FORM_VALUE, key);
            Assert.Contains(PATH, key);
        }

        [Fact]
        public void Default_keyed_by_form_query_accept()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(FORM), nameof(ACCEPT), nameof(QUERY) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
                    body: null,
                    protocol: PROTOCOL,
                    query: QUERY);

            request.Form[FORM] = FORM_VALUE;

            //Act
            string key = keyGen.Get(request);

            //Assert
            Assert.Contains(ACCEPT, key);
            Assert.Contains(QUERY, key);
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
                    headers: acceptHeader,
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
        public void Default_keyed_by_form_header_query_without_form_key()
        {
            //Arrange
            var keyGen = new DefaultCacheKeyGenerator(new string[] { nameof(QUERY), nameof(ACCEPT) });
            var request = new FakeRequest(
                    method: METHOD,
                    path: PATH,
                    headers: acceptHeader,
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
            Assert.DoesNotContain(FORM, key);
            Assert.DoesNotContain(FORM_VALUE, key);
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
                headers: acceptHeader);

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
