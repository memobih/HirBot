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
            return Ok(new { status = 200, data = respon.Data } );
        }
        //[Authorize]
        //[Route("update/{id}")]
        //public Task<IActionResult> update(string id)
        //{
        //    return Ok();
        //}
        [Authorize]
        [HttpPost("AddExperience")]
        public async  Task<IActionResult> create(ExperienceDto experience)
        {
            var respon = await experienceServices.AddExperienceAsync(experience);
            return StatusCode(respon.StatusCode); 
        }
        //[Authorize]
        //[Route("delete/{id}")]
        //public Task<IActionResult> delete(string id)
        //{
        //    return Ok();
        //}


    }
}
