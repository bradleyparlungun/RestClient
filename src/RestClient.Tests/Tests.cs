﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Rest.Serializers;
using Rest.Tests.TestRestServer;
using Should;
using Xunit;

namespace Rest.Tests
{
    public class Tests
    {
        [Fact]
        public async Task Should_get_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos");

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public async Task Should_get_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypes.ApplicationXml);

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos");

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public void Should_handle_error_when_getting_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypes.ApplicationXml);
                const string ErrorMessage = "error";

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/error/" + ErrorMessage).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.Message.ShouldEqual(ErrorMessage);
            }
        }

        [Fact]
        public void Should_handle_error_when_getting_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypes.ApplicationXml);
                const string ErrorMessage = "error";

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/error/" + ErrorMessage).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.Message.ShouldEqual(ErrorMessage);
            }
        }

        [Fact]
        public void Should_handle_not_found_when_getting_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypes.ApplicationJson);

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/unknown").Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_handle_not_found_when_getting_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypes.ApplicationXml);

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/unknown").Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_throw_exception_for_unkown_server_mediatype()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/foos");

                var aggregateException = Assert.Throws<AggregateException>(() =>
                {
                    var result = restClient.GetAsync<List<Foo>>("/api/foos").Result;
                    return result;
                });

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotAcceptable);
            }
        }

        [Fact]
        public void Should_throw_exception_for_unkown_client_mediatype()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypes.ApplicationJson);
                restClient.MediaTypeSerializers.Remove(restClient.MediaTypeSerializers.First(x => x is JsonMediaTypeSerializer));

                var aggregateException = Assert.Throws<AggregateException>(() =>
                {
                    var result = restClient.GetAsync<List<Foo>>("/api/foos").Result;
                    return result;
                });

                aggregateException.InnerException.ShouldBeType<NotSupportedException>();
            }
        }

        [Fact]
        public async Task Should_get_with_parameters()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var parameters = new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } };
                var foo = await restClient.GetAsync<Foo>("/api/foos", parameters);

                foo.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_get_with_id()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foo = await restClient.GetAsync<Foo>("/api/foos/{0}".FormatUri(1));

                foo.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_post_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), MediaTypes.ApplicationJson);

                id.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task Should_post_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), MediaTypes.ApplicationXml);

                id.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task Should_post_form_url_encoded_content()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var values = new List<KeyValuePair<string, string>>{};
                var content = new FormUrlEncodedContent(values);

                var id = await restClient.PostAsync<int>("/api/foos", content, MediaTypes.FormUrlEncoded);

                id.ShouldEqual(1);
            }
        }


        [Fact]
        public async Task Should_put_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var obj = await restClient.PutAsync<object>("/api/foos", new Foo(), MediaTypes.ApplicationJson);
                
                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_put_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var obj = await restClient.PutAsync<object>("/api/foos", new Foo(), MediaTypes.ApplicationXml);
                
                obj.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_put_form_url_encoded_content()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                var values = new List<KeyValuePair<string, string>> { };
                var content = new FormUrlEncodedContent(values);

                var obj = await restClient.PutAsync<object>("/api/foos", content, MediaTypes.FormUrlEncoded);

                obj.ShouldBeNull();
            }
        }
    }
}