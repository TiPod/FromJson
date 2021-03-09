using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace FromJson.Tests.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class StringController:ControllerBase
    {
        [HttpPost]
        public string PostStringInArgument([FromJson]string text)
        {
            return text;
        }

        [HttpPut]
        public string PutStringInArgument([FromJson] string text)
        {
            return text;
        }


        [HttpPost]
        public string PostStringInUpperArgument([FromJson(ignoreCase:true)] string Text)
        {
            return Text;
        }


    }
}
