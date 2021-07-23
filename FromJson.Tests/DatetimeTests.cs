using FromJson.Tests.Basic;
using FromJson.Tests.Controllers;
using NUnit.Framework;
using System;

namespace FromJson.Tests
{
    public class DatetimeTests : Server
    {
        private readonly string ControllerName = "Datetime";

        [Test]
        public void DatetimeInArgumentISO()
        {
            var server = GetTestServer();
            string date = DateTime.Now.ToString();
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(DatetimeController.Date)}", ParseJsonContent(new
                {
                    date
                })).Result;
                Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd"), res.Content.ReadAsStringAsync().Result);
            }
            Assert.Pass();
        }

        [Test]
        public void DatetimeInArgumentDateOnly()
        {
            var server = GetTestServer();
            string date = "2021-03-09";
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(DatetimeController.Date)}", ParseJsonContent(new
                {
                    date
                })).Result;
                Assert.AreEqual(date, res.Content.ReadAsStringAsync().Result);
            }
            Assert.Pass();
        }

        [Test]
        public void DatetimeInArgumentWithMinutes()
        {
            var server = GetTestServer();
            string date = "2021-03-09T12:01:20";
            using (var client = server.CreateClient())
            {
                var res = client.PostAsync($"/{ControllerName}/{nameof(DatetimeController.Datetime)}", ParseJsonContent(new
                {
                    date
                })).Result;
                Assert.AreEqual(date, res.Content.ReadAsStringAsync().Result);
            }
            Assert.Pass();
        }
    }
}