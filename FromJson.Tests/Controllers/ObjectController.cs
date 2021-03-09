﻿using FromJson.Tests.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace FromJson.Tests.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ObjectController:ControllerBase
    {

        public bool CheckObj([FromJson]TestModel model)
        {
            if (string.IsNullOrEmpty(model.Text))
            {
                return false;
            }
            return true;
        }

    }
}
