using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.APi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetInterviewDto>>> GetAll()
        {
            var interviews = await _interviewService.GetAllAsync();
            if (interviews == null)
            {
                return NotFound("No interviews found.");
            }

            return Ok(interviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetInterviewDto>> GetById(string id)
        {

            var response = await _interviewService.GetByIdAsync(id);
           if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { status = false, message = response.Message, errors = response.Errors });
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] InterviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid data. Please check the input and try again.", Details = ModelState });
            }

            var response = await _interviewService.CreateAsync(dto);
            if (!response.Succeeded)
            {
                return StatusCode(response.StatusCode, new { status = false, message = response.Message, errors = response.Errors });
            }
            return StatusCode(response.StatusCode, new
            {
                status = response.StatusCode == 200,
                message = response.Message,
                data = response.Data,
                errors = response.Errors
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, InterviewDto dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid data. Please check the input and try again.", Details = ModelState });
            }

            var response = await _interviewService.UpdateAsync(id, dto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { status = false, message = response.Message, errors = response.Errors });
            }
            else
            {
                return Ok(new { status = true, message = response.Message, data = response.Data });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            var deleted = await _interviewService.DeleteAsync(id);
            if (!deleted.Succeeded)
            {
                return NotFound($"Interview with ID {id} not found.");
            }
            else
            {
                return Ok(new { status = true, message = deleted.Message });
            }

        }
        [HttpGet("candidate")]
        public async Task<ActionResult<InterviewCandidateinfoDto>> GetInterviewCandidateInfo([FromQuery] string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
            {
                return BadRequest("Invalid Application ID. ID cannot be null or empty.");
            }

            var interviewInfo = await _interviewService.GetInterviewCandidateInfoAsync(applicationId);
            if (interviewInfo.Succeeded == false)
            {
                return NotFound($"Interview information for Application ID {applicationId} not found.");
            }

            return Ok(interviewInfo);
        }
        [HttpGet("exam/{interviewId}")]
        public async Task<ActionResult<object>> GetExamByInterviewId(string interviewId)
        {
            if (string.IsNullOrEmpty(interviewId))
            {
                return BadRequest("Invalid Interview ID. ID cannot be null or empty.");
            }

            var result = await _interviewService.GetExamByInterviewIdAsync(interviewId);
            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, new { status = false, message = result.Message, errors = result.Errors });
            }
            
            return Ok(result);
        }
    }
}