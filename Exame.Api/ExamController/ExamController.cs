using Exame.Services.DataTransferObjects;
using Exame.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
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
        public async Task<IActionResult> GetExame(int id , int level )
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var response = await _exameServices.GetALLExams();
            if (response.StatusCode == 200)
                return Ok(new { status = true, data = response.Data });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateExam( ExameDto exame)
        {
            var response = await _exameServices.CreateExame(exame);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [Authorize]
        [HttpDelete("{exameID}/delete")]
        public async Task<IActionResult> DeleteExam(int exameID)
        {
            var response = await _exameServices.DeleteExame(exameID);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }


        [Authorize]
        [HttpPut("{exameID}/edit")]
        public async Task<IActionResult> EditExam(int exameID, ExameDto exame)
        {
            var response = await _exameServices.EditExame(exameID, exame);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExame(int id)
        {
            var response = await _exameServices.GetExame(id);
            if (response.StatusCode == 200)
                return Ok(new { status = true, data = response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
    }
}
