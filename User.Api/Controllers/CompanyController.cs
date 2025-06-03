using HirBot.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Interfaces;
using User.Services.Response;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ApiControllerBase
    { 

        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [Authorize]
        [HttpPut("{id}/changestatus")]
        public async Task<IActionResult> editStatus(string  id ,[Required(ErrorMessage ="this field is required ")]CompanyStatus status)
        {
            var response = await _companyService.ChangeStatus(status , id);
            if (response.StatusCode == 200)

                return Ok(new { status = true, response.Message });
            else return StatusCode(response.StatusCode, new { status = false, response.Message });
            

        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(List<string> ids)
        {
            var response = await _companyService.Delete( ids);
            if (response.StatusCode == 200)

                return Ok(new { status = true, response.Message });
            else return StatusCode(response.StatusCode, new { status = false, response.Message });


        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetALLCompanies(string?  search =null, CompanyStatus? status = null,  int page=1 , int perpage=10)
        {
            var response = await _companyService.GetAllCompanies(search ,status ,  page , perpage);
            if (response.StatusCode == 200)

                return Ok(new { status = true, response.Data });
            else return StatusCode(response.StatusCode, new { status = false, response.Message });


        }
        [HttpGet("companies")]
        public async Task<IActionResult> GetALLCompanies(string ?search=null , int page=1 , int perpage=10)
        {
            var response = await _companyService.GetAcceptedCompanies( search , page , perpage);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Data });
            else return StatusCode(response.StatusCode, new { status = false, response.Message });


        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _companyService.GetByid(id);
            if (response.StatusCode == 200)

                return Ok(new { status = true, response.Data });
            else return StatusCode(response.StatusCode, new { status = false, response.Message });


        }
        [Authorize]
        [HttpPut("basic-info")]
        public async Task<IActionResult> EditBasicInformation([FromBody] CompanyBasicInformationDto dto)
        {
            var response = await _companyService.EditBasicInformation(dto);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message, response.Data });
            else
                return StatusCode(response.StatusCode, new { status = false, response.Message });
        }

        [Authorize]
        [HttpPut("social-media")]
        public async Task<IActionResult> EditSocialMedia([FromBody] SocialMedia dto)
        {
            var response = await _companyService.EditSocialMedia(dto);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message, response.Data });
            else
                return StatusCode(response.StatusCode, new { status = false, response.Message });
        }
        [Authorize]
        [HttpPut("contact-info")]
        public async Task<IActionResult> EditContactInfo([FromBody] Contact dto)
        {
            var response = await _companyService.EditContactInfo(dto);
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message, response.Data });
            else
                return StatusCode(response.StatusCode, new { status = false, response.Message });
        }

        [Authorize]
        [HttpGet("basic-info")]
        public async Task<IActionResult> GetBasicInformation()
        {
            var response = await _companyService.GetBasicInformation();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message, response.Data });
            else
                return StatusCode(response.StatusCode, new { status = false, response.Message });
        }

        [Authorize]
        [HttpGet("social-media")]
        public async Task<IActionResult> GetSocialMedia()
        {
            var response = await _companyService.GetSocialMedia();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message, response.Data });
            else
                return StatusCode(response.StatusCode, new { status = false, response.Message });
        }

        [Authorize]
        [HttpGet("contact-info")]
        public async Task<IActionResult> GetContactInfo()
        {
            var response = await _companyService.GetContactInfo();
            if (response.StatusCode == 200)
                return Ok(new { status = true, response.Message, response.Data });
            else
                return StatusCode(response.StatusCode, new { status = false, response.Message });
        }
    }
}
