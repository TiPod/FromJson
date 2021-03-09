using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace FromJson.Tests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController:ControllerBase
    {
        [HttpPost]
        public string TestString([FromJson]string text)
        {

            return text;
        }
    }
}
