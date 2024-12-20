using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Authencation;
using Project.Services.Interfaces;


namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 

    public class UserController :ApiControllerBase
    {
        private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;
       public UserController(Project.Services.Interfaces.IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [Route("AddUser")]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserRegisterDto adduserDto)
        {
            var response = await _authenticationService.AddUser(adduserDto);

            return StatusCode(response.StatusCode, response);
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> login(LoginDto request)
        {
            var response= await _authenticationService.Login(request); 
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        [Authorize]
        public  IActionResult te()
        {
            return Ok("good");
        }
    }
}