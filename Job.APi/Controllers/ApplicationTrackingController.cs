using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;

namespace Job.APi.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class ApplicationTrackingController : ApiControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationTrackingController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }
        [HttpGet("approved")]
        public async Task<IActionResult> TrackApprovedApplications()
        {
            var response = await _applicationService.TrackApprovedApplications();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [HttpGet("rejected")]
        public async Task<IActionResult> TrackRejectedApplications()
        {
            var response = await _applicationService.TrackRejectedApplications();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [HttpGet("inprogress")]
        public async Task<IActionResult> TrackInprogressApplications()
        {
            var response = await _applicationService.TrackInprogressApplications();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [HttpPost("startprocess/{applicationId}")]
        public async Task<IActionResult> StartProcess(int applicationId)
        {
            var response = await _applicationService.StartProcess(applicationId);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [HttpGet("all")]
        public async Task<IActionResult> TrackAllApplications()
        {
            var response = await _applicationService.TrackAllApplications();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
    }
}