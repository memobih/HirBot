using AutoApply.Services.Interfaces;
using HirBot.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace AutoApply.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoApplyController : ControllerBase
    {
        private readonly IAutoApplyService _autoApplyService;
        public AutoApplyController (IAutoApplyService autoApplyService)
        {
            _autoApplyService = autoApplyService;
        }
        [Authorize]

        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeStatus([FromQuery] bool isAuto)
        {
            var response = await _autoApplyService.ChangeStatus(isAuto);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [Authorize]

        [HttpGet("jobs-by-autoapply")]
        public async Task<IActionResult> GetJobByAutoApply(string? search = null, string? experience = null, string? location = null, [FromQuery] List<LocationType>? locationType = null, [FromQuery] List<EmployeeType>? JobType = null, int page = 1, int perpage = 10, int? minSalary = null, int? maxSalary = null)
        {
            var response = await _autoApplyService.GetJobByAutoApply(
            search, experience, location, locationType, JobType, page, perpage, minSalary, maxSalary);

            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [Authorize]
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var response = await _autoApplyService.GetStatus();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }

    }
}
