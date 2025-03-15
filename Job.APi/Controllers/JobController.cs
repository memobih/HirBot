using HirBot.Data.Enums;
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getALLJop(string? search = null, JobStatus? status = null, LocationType? locationType = null, EmployeeType? JobType = null, int page = 1, int perPage = 10, string? sort = "salary", string sortDirection=null)
        {
            var response = await _jobService.getAllJobs(search , status , locationType , JobType , page , perPage , sort , sortDirection);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpGet("drafted")]
        public async Task<IActionResult> getALLdraftedJob(string? search = null, JobStatus? status = null, LocationType? locationType = null, EmployeeType? JobType = null, int page = 1, int perPage = 10 , string? sort = "salary", string sortDirection = null)
        {
            var response = await _jobService.getAllDraftedJobs(search, status, locationType, JobType, page, perPage, sort, sortDirection);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobByID(int id)
        {
            var response = await _jobService.JobDetails( id);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteRanges(List<int> ids)
        {
            var response = await _jobService.DeleteJobs(ids);
            if (response.StatusCode == 200)
                return Ok(new { status = true , response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpGet("recomendation")]
        public async Task<IActionResult> GetRecemoendationJobs(string? search = null, string? experience = null, string? location = null, [FromQuery] List<LocationType>? locationType = null, [FromQuery] List<EmployeeType>? JobType = null, int page = 1, int perpage = 10, int? minSalary = null, int? maxSalary = null)
        {
            var response = await _jobService.GetJosRecomendations(search, experience, location, locationType, JobType, page, perpage, minSalary, maxSalary);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpPut("status/{id}")]
        public async Task<IActionResult> EditJobStatus(int id, EditStatusDto status)
        {
            var response = await _jobService.EditJobStatus(id, status);
            if (response.StatusCode == 200)
                return Ok(new { status = true, massage = response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }

    }
}
