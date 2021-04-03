using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServiceAPI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceAPI1Controller : ControllerBase
    {
        [HttpGet("Hello")]
        public IActionResult Hello()
        {
            
            return Content("Hello Service1===="+ Request.HttpContext.Connection.LocalPort );
        }

        [HttpGet("Health")]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
