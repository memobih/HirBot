using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Comman.Idenitity;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project.ResponseHandler.Models;
using User.Services.DataTransferObjects;
using User.Services.DataTransferObjects.Authencation.ProfileDto;
using User.Services.Interfaces;

namespace User.Api.Controllers.Portfolio
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalInformationController : ApiControllerBase
    {
        private readonly IPersonalInformationService _personalInformationService; 
        public PersonalInformationController(IPersonalInformationService personalInformationService)
        {
            _personalInformationService= personalInformationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPersonalInformation()
        {
            var respone = await _personalInformationService.GetPersonalInformationAsync();
            if (respone.StatusCode == 200)
                return Ok(new { status = true, respone.Data });
            return BadRequest("there are error accured");

        }

        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> UpdatePersonalInformationAsync(PersonalInfoDto personalInfoDto)
        {
            var respone = await _personalInformationService.UpdatePersonalInformationAsync(personalInfoDto);
            if (respone.StatusCode == 200)
                return Ok(new { status = true,  Data= personalInfoDto });
            return BadRequest("there are error accured");
        }


    }
}