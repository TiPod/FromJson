using FromJson.Tests.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FromJson.Tests.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class NumberController : ControllerBase
    {
        [HttpPost]
        public int AddTen([FromJson] int number)
        {
            return number + 10;
        }

        [HttpPost]
        public decimal AddTenDecimal([FromJson] decimal number)
        {
            return number + 10;
        }

        [HttpPost]
        public string PostEnum([FromJson] TestEnum type)
        {
            return Enum.GetName(typeof(TestEnum), type);
        }
    }
}