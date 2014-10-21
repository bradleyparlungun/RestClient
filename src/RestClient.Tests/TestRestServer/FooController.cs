﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Rest.Tests.TestRestServer
{
    [RoutePrefix("api/foos")]
    public class FooController : ApiController
    {
        [Route("{id}")]
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, CreateFoo());
        }

        [Route("")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new List<Foo> { CreateFoo(), CreateFoo(), CreateFoo() });
        }

        private static Foo CreateFoo()
        {
            return new Foo
            {
                SomeInt = 1,
                SomeString = "some",
                Bar = new Bar
                {
                    SomeInt = 1,
                    SomeString = "some"
                }
            };
        }
    }

    public class Foo
    {
        public string SomeString { get; set; }
        public int SomeInt { get; set; }
        public Bar Bar { get; set; }
    }

    public class Bar
    {
        public string SomeString { get; set; }
        public int SomeInt { get; set; }
    }
}