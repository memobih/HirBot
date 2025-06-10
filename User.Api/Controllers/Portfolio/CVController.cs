using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.images;
using User.Services.Interfaces;

namespace User.Api.Controllers.Portfolio
{
    [Route("api/[controller]")]
    [ApiController]
    public class CVController : ApiControllerBase
    {

        private readonly ICVService _cvService; 
        public CVController(ICVService cVService) { 
        _cvService = cVService;
        }
        [HttpPost("UpdateCV")]
        [Authorize]
        public async  Task<IActionResult> UpdateCV(FileDto CV)
        {
            var result=await _cvService.UpdateCv(CV); 
            if (result.StatusCode==200) return Ok(new { status = true, massage = "the cv updated sucsseful"  , data=result.Data});
            else return BadRequest(new { status = false, massage =result.Message });
        }
        [HttpDelete("DeleteCV")]
        [Authorize]
        public async Task<IActionResult> DeleteCV()
        {
            var result = await _cvService.DeleteCv();
            if (result) return Ok(new { status = true, massage = "the cv deleted sucsseful" });
            else return BadRequest(new { status = false, massage = "there are error accured" });
        }
    }
}
