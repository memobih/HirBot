using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.APi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
       public IInterviewService _interviewService;
        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }
        [HttpPost("ScheduleInitialInterview")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> ScheduleInitialInterview([FromBody] InitialInterviewDto interviewDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _interviewService.SchuduleInitialInterview(interviewDto);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("ScheduleTechnicalInterview")]
        [Authorize]
        public async Task<IActionResult> ScheduleTechnicalInterview([FromBody] TechnicalInterviewDto interviewDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _interviewService.SchuduleTechnicalInterview(interviewDto);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }
}