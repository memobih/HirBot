using HirBot.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.Interfaces;

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
        public async Task<IActionResult> editStatus(string  id , CompanyStatus status)
        {
            var response = await _companyService.ChangeStatus(status , id);
            if (response.StatusCode == 200)

                return Ok(new { status = true, response.Message });
            else return StatusCode(response.StatusCode, new { status = false, response.Message });
            

        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _companyService.Delete( id);
            if (response.StatusCode == 200)

                return Ok(new { status = true, response.Message });
            else return StatusCode(response.StatusCode, new { status = false, response.Message });


        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetALLCompanies(string?  search =null, int page=1 , int perpage=10)
        {
            var response = await _companyService.GetAllCompanies(search , page , perpage);
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
    }
}
