using FromJson.Tests.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FromJson.Tests.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ArrayController : ControllerBase
    {
        [HttpPost]
        public int StringList([FromJson] List<string> idList)
        {
            return idList.Count;
        }

        [HttpPost]
        public int NumberList([FromJson] List<int> numberList)
        {
            return numberList.Sum();
        }

        [HttpPost]
        public int ObjectList([FromJson] List<TestModel> modelList)
        {
            return modelList.Count;
        }
    }
}