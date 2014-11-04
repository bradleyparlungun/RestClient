﻿using System.Net.Http;

namespace Rest.Client
{
    public static class HttpVerb
    {
        private static readonly HttpMethod patch = new HttpMethod("PATCH");

        public static HttpMethod Patch
        {
            get { return patch; }
        }
    }
}