using FromJson.Tests.Basic;
using FromJson.Tests.Controllers;
using NUnit.Framework;

namespace FromJson.Tests
{
    public class StringTests : Server
    {
        private string ControllerName = "String";

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
                Assert.AreEqual(text, res.Content.ReadAsStringAsync().Result);
            }
            Assert.Pass();
        }

        [Test]
        public void PostStringInUpperArgument()
        {
            var server = GetTestServer();
            string text = "test Text Content";
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(StringController.PostStringInUpperArgument)}", ParseJsonContent(new
                {
                    text = text
                })).Result;
                Assert.AreEqual(text, res.Content.ReadAsStringAsync().Result);
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
                Assert.AreEqual(text, res.Content.ReadAsStringAsync().Result);
            }
            Assert.Pass();
        }
    }
}