using Azure.Storage.Blobs.Models;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using Jop.Services.Implemntations;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Prepare;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Job.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ApiControllerBase
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }
        [Authorize]
        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetALLApplications(int jobId, string? search = null, ApplicationStatus? status = null, string sort = "score", string? sortDirection = null, int page = 1, int perpage = 10)
        {
            var response = await _applicationService.GetALLAplications(jobId, search, status, sort, sortDirection, page, perpage);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpPost("apply/{jobId}")]
        public async Task<IActionResult> ApplyyOnJob(int jobId)
        {
            var response = await _applicationService.ApplicateOnJob(jobId);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpPut("approve")]
        public async Task<IActionResult> approved(List<int> ids)
        {
            var response = await _applicationService.ApproveApplocations(ids);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpPut("rejected")]
        public async Task<IActionResult> rejected(List<int> ids)
        {
            var response = await _applicationService.RejectApplication(ids);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            var response = await _applicationService.DeleteApplications(ids);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpGet("{jobId}/approved")]
        public async Task<IActionResult> GetAllApprovedApplications(int jobId, string? search = null, string sort = "score", string? sortDirection = null, int page = 1, int perpage = 10)
        {
            var response = await _applicationService.GetAllApprovedApplications(jobId, search, sort, sortDirection, page, perpage);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
     
        [HttpPut("accept")]
        public async Task<IActionResult> AcceptTheApplication(int ApplicationId)
        {
            var response = await _applicationService.AcceptTheApplication(ApplicationId);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
        [Authorize]
        [HttpPut("reject")]
        public async Task<IActionResult> RejectTheApplication(int ApplicationId)
        {
            var response = await _applicationService.RejectTheApplication(ApplicationId);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message });
            return StatusCode(response.StatusCode, new { status = false, massage = response.Message });
        }
      

    }
}
