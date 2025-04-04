using Exame.Services.DataTransferObjects;
using Exame.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Api.ExamController
{
    [Route("api/[controller]")]
    [ApiController]
    public  class ExamController : ApiControllerBase
    {
        private readonly IExameService _exameServices; 
        public ExamController(IExameService exameServices)
        {
            _exameServices = exameServices;
        }
        [Authorize]
        [HttpGet("{id}/getexame")]
        public async Task<IActionResult> GetExame(int id)
        {
            var response=await _exameServices.DoExame(id);
            if(response.StatusCode==200) 
                return Ok(new {status=true , message=response.Message ,data=response.Data});
            return StatusCode(response.StatusCode , new { status = false, message = response.Message });
        }
        [Authorize]
        [HttpPost("{id}/finishExame")]
        public async Task<IActionResult> finishExame(int id , List<AnswerDto>answers)
        {
            var response = await _exameServices.FinishExame(id , answers);
            if (response.StatusCode == 200)
                return Ok(new { status = true,  data = response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
    }
}
