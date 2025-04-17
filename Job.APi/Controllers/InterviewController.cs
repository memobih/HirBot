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
            if (interviews == null )
            {
                return NotFound("No interviews found.");
            }

            return Ok(interviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetInterviewDto>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID. ID cannot be null or empty.");
            }
            var interview = await _interviewService.GetByIdAsync(id);
            if (interview == null)
            {
                return NotFound($"Interview with ID {id} not found.");
            }

            return Ok(interview);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] InterviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid data. Please check the input and try again.", Details = ModelState });
            }

            var response = await _interviewService.CreateAsync(dto);
            if(!response.Succeeded)
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
        public async Task<ActionResult> Update(string id, [FromBody] InterviewDto dto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID. ID cannot be null or empty.");
            }

            if (!ModelState.IsValid)
            {
                 return BadRequest(new { Error = "Invalid data. Please check the input and try again.", Details = ModelState });
            }

            var response = await _interviewService.UpdateAsync(id, dto);
            if (response == APIOperationResponse<GetInterviewDto>.NotFound())
            {
                return NotFound($"Interview with ID {id} not found.");
            }
            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { status = false, message = response.Message, errors = response.Errors });
            }
            else 
            {
                return Ok(new { status = true, message = response.Message, data = response.Data });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID. ID cannot be null or empty.");
            }

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
        public async Task<ActionResult<InterviewCandidateinfoDto>> GetInterviewCandidateInfo([FromQuery]string applicationId)
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
    }
}