using FromJson.Tests.Basic;
using FromJson.Tests.Controllers;
using FromJson.Tests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FromJson.Tests
{
    public class ArrayTests : Server
    {
        private string ControllerName = "Array";

        [Test]
        public void PostStringInArgument()
        {
            var server = GetTestServer();
            List<string> req = new List<string>()
            {
                "1",
                "2",
                "3",
                "4"
            };
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(ArrayController.StringList)}", ParseJsonContent(new
                {
                    idList = req
                })).Result;
                Assert.AreEqual(req.Count, int.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }

        [Test]
        public void PostNumberInArgument()
        {
            var server = GetTestServer();
            List<int> req = new List<int>()
            {
                1,
                2,
                3,
                4
            };
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(ArrayController.NumberList)}", ParseJsonContent(new
                {
                    numberList = req
                })).Result;
                Assert.AreEqual(req.Sum(), int.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }

        [Test]
        public void PostObjectInArgument()
        {
            var server = GetTestServer();
            List<TestModel> req = new List<TestModel>()
            {
                new TestModel()
                {
                    CreateAt = DateTime.Now,
                    Status = 2,
                    Text = "23333"
                },
                new TestModel()
                {
                    CreateAt = DateTime.Now,
                    Status = 2,
                    Text = "23333"
                }
            };
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(ArrayController.ObjectList)}", ParseJsonContent(new
                {
                    modelList = req
                })).Result;
                Assert.AreEqual(req.Count, int.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }
    }
}