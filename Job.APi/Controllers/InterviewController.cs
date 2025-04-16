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
        public async Task<ActionResult<APIOperationResponse<List<Interview>>>> GetAll()
        {
            var response = await _interviewService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIOperationResponse<Interview>>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new APIOperationResponse<Interview>
                {
                    StatusCode = 400,
                    Message = "Invalid ID. ID must be greater than 0."
                });
            }

            var response = await _interviewService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<ActionResult<APIOperationResponse<Interview>>> Create([FromBody] InterviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new APIOperationResponse<Interview>
                {
                    StatusCode = 400,
                    Message = "Invalid data. Please check the input and try again."
                });
            }

            var response = await _interviewService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<APIOperationResponse<Interview>>> Update(int id, [FromBody] InterviewDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new APIOperationResponse<Interview>
                {
                    StatusCode = 400,
                    Message = "Invalid ID. ID must be greater than 0."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new APIOperationResponse<Interview>
                {
                    StatusCode = 400,
                    Message = "Invalid data. Please check the input and try again."
                });
            }

            var response = await _interviewService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<APIOperationResponse<bool>>> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new APIOperationResponse<bool>
                {
                    StatusCode = 400,
                    Message = "Invalid ID. ID must be greater than 0."
                });
            }

            var response = await _interviewService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}