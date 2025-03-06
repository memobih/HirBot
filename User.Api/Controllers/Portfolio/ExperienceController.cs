using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Implemntation;
using User.Services.Interfaces;

namespace User.Api.Controllers.Portfolio
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController : ApiControllerBase
    {
        private readonly IExperienceServices experienceServices; 
       public  ExperienceController (IExperienceServices experienceServices)
        {
            this.experienceServices = experienceServices;

        }
        [Authorize]
        [  HttpGet("GetAllExperience")]
        public async  Task<IActionResult> index( )
        {
            var respon = await experienceServices.GetExperienceAsync();
            return Ok(new { status = true, data = respon.Data } );
        }
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> update(int id , ExperienceDto experience)
        {
            var response = await experienceServices.EditExperienceAsync(id , experience);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = "experience is updated successuful" });
            return StatusCode(response.StatusCode, new { status = false,message= response.Message });
        }
        [Authorize]
        [HttpPost("AddExperience")]
        public async  Task<IActionResult> create(ExperienceDto experience)
        {
            var respon = await experienceServices.AddExperienceAsync(experience);
            return StatusCode(respon.StatusCode); 
        }
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> delete(int id )
        { 
            var response = await experienceServices.DeleteExperienceAsync(id );
            if (response.StatusCode == 200)
                return Ok(new {status=true , message ="experience is delted successuful"});
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> show(int id)
        {
            var response = await experienceServices.GetExperienceAsyncByid(id);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }



    }
}
