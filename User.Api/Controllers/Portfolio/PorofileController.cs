﻿using Microsoft.AspNetCore.Authorization;
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
    public  class PorofileController : ApiControllerBase
    {  
       private readonly IPorofileService _profileService; 
        public PorofileController (IPorofileService pofileService)
        {
            _profileService = pofileService;
        }
        [Authorize]
        [HttpGet] 
        public async Task<IActionResult> GetPorofile()
        {
            var respon=await _profileService.GetPorofileAync();
            if(respon.StatusCode==200) 
                return Ok(new { status = true, respon.Data } );
            else 
                return StatusCode(respon.StatusCode ,new {status=false , massage="User not found"});
        }
        [Authorize]
        [HttpGet("cv")]
        public async Task<IActionResult> GetCV()
        {
            var respon = await _profileService.GetCvAync();
            if (respon.StatusCode == 200)
                return Ok(new { status = true, respon.Data });
            else
                return StatusCode(respon.StatusCode, new { status = false, massage = "User not found" });
        }
        [Authorize]
        [HttpGet("Company")]
        public async Task<IActionResult> GetCompanyProfile()
        {
            var respon = await _profileService.GetCompanyProfileAsync();
            if (respon.StatusCode == 200)
                return Ok(new { status = true, respon.Data });
            else
                return StatusCode(respon.StatusCode, new { status = false, respon.Message } );
        }
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetProfileByUserName(string userName)
        {
            var respon = await _profileService.GetProfileWithUserName(userName);
            if (respon.StatusCode == 200)
                return Ok(new { status = true, respon.Data });
            else
                return StatusCode(respon.StatusCode, new { status = false, respon.Message });
        }
        [Authorize]
        [HttpPut("update/{id}/CurrentJob")]
        public async Task<IActionResult> UpdateCurrentJob(int id )
        {
            var respon = await _profileService.UpdateCurrentJob(id);
            if (respon.StatusCode == 200)
                return Ok(new { status = true, respon.Message });
            else
                return StatusCode(respon.StatusCode, new { status = false, respon.Message });
        }
        [Authorize]
        [HttpDelete("CurrentJob")]
        public async Task<IActionResult> DeleteCurrentJob()
        {
            var respon = await _profileService.DeleteCurrentJob();
            if (respon.StatusCode == 200)
                return Ok(new { status = true, respon.Message });
            else
                return StatusCode(respon.StatusCode, new { status = false, respon.Message });
        }


    }
}
