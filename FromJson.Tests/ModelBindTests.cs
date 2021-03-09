using FromJson.Tests.Basic;
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
    public class Tests: Server
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            var server = GetTestServer();
            using (var client = server.CreateClient())
            {

                var res = client.PostAsync("/Test", ParseJsonContent(new
                {
                    text = "text text"
                })).Result;
                //res.Content.ReadAsStringAsync().Result 
                Assert.AreEqual(res.Content.ReadAsStringAsync().Result, "text text");
            }
            Assert.Pass();
        }


    }
}