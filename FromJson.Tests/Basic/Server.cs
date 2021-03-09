using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FromJson.Tests.Basic
{
    public class Server
    {
        private static TestServer _server = null;
        public TestServer GetTestServer()
        {
            if(_server != null)
            {
                return _server;
            }
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);
            _server = server;
            return _server;
        }

        public StringContent ParseJsonContent(object obj)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return stringContent;
        }

    }
}
