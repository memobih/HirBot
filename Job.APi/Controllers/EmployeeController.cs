using HirBot.Data.Enums;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ApiControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Add(AddEmployeeDto employee)
        {
            var response = await _employeeService.AddEmployee(employee);
            if (response.StatusCode==200)
            {
                return Ok(new {status=true , response.Message});
            }
            return StatusCode(response.StatusCode , new { status = false, response.Message , errors=response.Errors });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AllEmployee(string? search = null, EmployeeType? jobType = null, LocationType? workType = null, int? rate = null)
        {
            var response = await _employeeService.GetAllEmployee(search , jobType , workType , rate );
            if (response.StatusCode == 200)
            {
                return Ok(new { status = true, response.Message , response.Data });
            }
            return StatusCode(response.StatusCode, new { status = false, response.Message, errors = response.Errors });
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> show(int id )
        {
            var response = await _employeeService.showEmployee(id);
            if (response.StatusCode == 200)
            {
                return Ok(new { status = true, response.Message, response.Data });
            }
            return StatusCode(response.StatusCode, new { status = false, response.Message, errors = response.Errors });
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> delete(List<int> ids)
        {
            var response = await _employeeService.DeleteEmployees(ids);
            if (response.StatusCode == 200)
            {
                return Ok(new { status = true, response.Message });
            }
            return StatusCode(response.StatusCode, new { status = false, response.Message, errors = response.Errors });
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> edit(int id , AddEmployeeDto employee)
        {
            var response = await _employeeService.EditEmployee(id , employee);
            if (response.StatusCode == 200)
            {
                return Ok(new { status = true, response.Message });
            }
            return StatusCode(response.StatusCode, new { status = false, response.Message, errors = response.Errors });
        }
    }
}
