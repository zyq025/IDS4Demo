using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "orderScope")]
        public IActionResult Orders()
        {
            string[] orders = { "order1", "order2" , "order3" };
            return Ok(orders);
        }
    }
}
