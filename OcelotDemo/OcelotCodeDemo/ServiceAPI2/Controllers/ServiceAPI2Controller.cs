using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServiceAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceAPI2Controller : ControllerBase
    {
        [HttpGet("Hello")]
        public IActionResult Hello()
        {
            return Content("Hello Service2");
        }
        [HttpGet("Health")]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
