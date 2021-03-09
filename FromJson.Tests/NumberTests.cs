using FromJson.Tests.Basic;
using FromJson.Tests.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FromJson.Tests
{

    public class NumberTests:Server
    {
        string ControllerName = "Number";

        [Test]
        public void NumberInArgument()
        {
            var server = GetTestServer();
            int number = 114514;
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(NumberController.AddTen)}", ParseJsonContent(new
                {
                    number = number
                })).Result;
                Assert.AreEqual( 114514 + 10, int.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }
        [Test]
        public void NumberInArgumentDecimal()
        {
            var server = GetTestServer();
            decimal number = 11451400000;
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(NumberController.AddTenDecimal)}", ParseJsonContent(new
                {
                    number = number
                })).Result;
                Assert.AreEqual(11451400000 + 10, decimal.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }

        [Test]
        public void Enum()
        {
            var server = GetTestServer();
            
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(NumberController.PostEnum)}", ParseJsonContent(new
                {
                    type = 1
                })).Result;

                Assert.AreEqual("SqlServer", res.Content.ReadAsStringAsync().Result);
            }
            Assert.Pass();
        }

        [Test]
        public void EmptyEnum()
        {
            var server = GetTestServer();

            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(NumberController.PostEnum)}", ParseJsonContent(new
                {
                    type = -11
                })).Result;

                Assert.AreEqual("", res.Content.ReadAsStringAsync().Result);
            }
            Assert.Pass();
        }

    }
}
