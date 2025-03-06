using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class JobController : ApiControllerBase
    {
        private readonly IJobService _jobService;
        public JobController (IJobService jobService)
        {
            _jobService = jobService;
        }
        [Authorize]
        [HttpPost("create")] 
        public async Task<IActionResult> AddJob(AddJobDto job)
        {
           var response = await _jobService.AddJop(job);
            if (response.StatusCode == 200)
                return Ok(new { status = true , massage=response.Message });
            return StatusCode(response.StatusCode , new { status = false, massage = response.Message });

        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> editJob(int id , AddJobDto job)
        {
            var response = await _jobService.edit(id , job);
            if (response.StatusCode == 200)
                return Ok(new { status = true, massage = response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }

    }
}
