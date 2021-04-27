using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [HttpGet("CacheTest")]
        public IActionResult CacheTest()
        {
            return Content("CacheTest====" + DateTime.Now);
        }

        [HttpGet("TimeoutTest")]
        public IActionResult TimeoutTest()
        {
            Thread.Sleep(10000); //模拟业务处理需要10秒
            return Content("TimeoutTest====");
        }


        [HttpGet("Health")]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
