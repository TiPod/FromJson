using FromJson.Tests.Basic;
using FromJson.Tests.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Text;

namespace FromJson.Tests
{
    public class StringTests: Server
    {
        string ControllerName = "String";
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void PostStringInArgument()
        {
            var server = GetTestServer();
            string text = "test Text Content";
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(StringController.PostStringInArgument)}", ParseJsonContent(new
                {
                    text = text
                })).Result;
                Assert.AreEqual(res.Content.ReadAsStringAsync().Result, text);
            }
            Assert.Pass();
        }

        [Test]
        public void PutStringInArgument()
        {
            var server = GetTestServer();
            string text = "test Text Content";
            using (var client = server.CreateClient())
            {
                var res = client.PutAsync($"/{ControllerName}/{nameof(StringController.PutStringInArgument)}", ParseJsonContent(new
                {
                    text = text
                })).Result;
                Assert.AreEqual(res.Content.ReadAsStringAsync().Result, text);
            }
            Assert.Pass();
        }





    }
}