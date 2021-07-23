using FromJson.Tests.Basic;
using FromJson.Tests.Controllers;
using FromJson.Tests.Models;
using NUnit.Framework;

namespace FromJson.Tests
{
    public class ObjectTests : Server
    {
        private readonly string ControllerName = "Object";

        [Test]
        public void ObjectInArgument()
        {
            var server = GetTestServer();
            var model = new TestModel()
            {
                Text = "bbbb",
                Detail = new SecondModel()
                {
                    Name = "Name!"
                }
            };
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(ObjectController.CheckObj)}", ParseJsonContent(new
                {
                    model
                })).Result;
                Assert.AreEqual(true, bool.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }

        [Test]
        public void BindObjectInArgument()
        {
            var server = GetTestServer();
            var model = new TestModel()
            {
                Text = "bbbb"
            };
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(ObjectController.BindModel)}", ParseJsonContent(new
                {
                    model,
                    Text = "aaaa"
                })).Result;
                Assert.AreEqual(true, bool.Parse(res.Content.ReadAsStringAsync().Result));
            }
            Assert.Pass();
        }
    }
}