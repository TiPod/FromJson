using FromJson.Tests.Basic;
using FromJson.Tests.Controllers;
using FromJson.Tests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FromJson.Tests
{
    public class ObjectTests:Server
    {
        string ControllerName = "Object";

        [Test]
        public void ObjectInArgument()
        {
            var server = GetTestServer();
            var model = new TestModel() { 
            Text ="bbbb"
            };
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(ObjectController.CheckObj)}", ParseJsonContent(new
                {
                    model = model
                })).Result;
                Assert.AreEqual(true,bool.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }


    }
}
