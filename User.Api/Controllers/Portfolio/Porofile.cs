using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.Interfaces;

namespace User.Api.Controllers.Portfolio
{

    [Route("api/[controller]")]
    [ApiController]
    public  class Porofile : ApiControllerBase
    {  
       private readonly IPorofileService _profileService; 
        public Porofile (IPorofileService pofileService)
        {
            _profileService = pofileService;
        }
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetPorofile(string id)
        {
            var respon=await _profileService.GetPorofileAync(id);
            if(respon.StatusCode==200) 
                return Ok(new { status = true, respon.Data } );
            else 
                return StatusCode(respon.StatusCode ,new {status=false , massage="User not found"});
        }
    }
}
