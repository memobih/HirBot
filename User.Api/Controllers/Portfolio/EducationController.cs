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
            return Ok(respon.Data);
        }
        //[Authorize]
        //[Route("update/{id}")]
        //public Task<IActionResult> update(string id)
        //{
        //    return Ok();
        //}
        [Authorize]
        [HttpPost("AddEducation")]
        public async Task<IActionResult> create(EducationDto education)
        {
            var respon = await educationService.AddEducationAsync(education);
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
