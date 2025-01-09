using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HirBot.Comman.Idenitity;
using Mailing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project.ResponseHandler.Models;
namespace Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TestController : ApiControllerBase
    {

        private readonly IMailingService _mailingService;
        public TestController(IMailingService mailingService )
        {
            _mailingService = mailingService;
        }
        [HttpGet]
        public IActionResult GetOk() {
            return Ok("hhh mahmad");
        } 

    }
}
