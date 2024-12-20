using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
namespace Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TestController : ApiControllerBase
    {
        [HttpGet]
        public IActionResult GetOk() {
            return Ok("evey thing i hope to be ok ");
        } 

    }
}
