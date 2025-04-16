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
        public async Task<ActionResult<GetInterviewDto>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
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
        public async Task<ActionResult> Update(int id, [FromBody] InterviewDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
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
                return StatusCode(response.StatusCode, new { status = false, message = response.Message });
            }
            else 
            {
                return Ok(new { status = true, message = response.Message, data = response.Data });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than 0.");
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
    }
}