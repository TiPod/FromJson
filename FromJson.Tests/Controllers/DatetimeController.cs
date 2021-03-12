using Microsoft.AspNetCore.Mvc;
using System;

namespace FromJson.Tests.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DatetimeController : ControllerBase
    {
        [HttpPost]
        public string Date([FromJson] DateTime? date)
        {
            return date.Value.ToString("yyyy-MM-dd");
        }

        [HttpPost]
        public string Datetime([FromJson] DateTime? date)
        {
            return date.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}