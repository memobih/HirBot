using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Interfaces;

namespace User.Api.Controllers.Portfolio
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationController : ApiControllerBase
    {

        private readonly IEducationService educationService;
        public EducationController(IEducationService educationService)
        {
            this.educationService = educationService;

        }
        [Authorize]
        [HttpGet("GetAllEducation")]
        public async Task<IActionResult> index()
        {
            var respon = await educationService.GetAllEducationAsync();
            return Ok(new { status = true, respon.Data } );
        }
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> update(int  id  , EducationDto education)
        {
            var respon=await  educationService.EditEducationAsync(id , education);
            return StatusCode(respon.StatusCode);
        }
        [Authorize]
        [HttpPost("AddEducation")]
        public async Task<IActionResult> create(EducationDto education)
        {
            var respon = await educationService.AddEducationAsync(education);
            return StatusCode(respon.StatusCode);
        }
        [Authorize]
        [HttpGet("view/{id}")]
        public async Task<IActionResult> get(int id)
        {
            var respon = await educationService.GetEducationByIdAsync(id);
            if(respon.StatusCode==200)
               return Ok(new { status = true, respon.Data });
           return StatusCode(respon.StatusCode , new { status =false  });
        }
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var respon = await educationService.DeleteEducationByIdAsync(id);
            if (respon.StatusCode == 200)
                return Ok();
            return StatusCode(respon.StatusCode, new { status = false });
        }

    }
}
