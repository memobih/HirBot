using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Job.APi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { status = true, message = "InterviewController is working" });
        }
    }
}