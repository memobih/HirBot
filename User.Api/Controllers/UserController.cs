using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using User.Services.DataTransferObjects.Authencation;
using HirBot.Redies;
using Microsoft.AspNetCore.Http;
using Azure;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web;
using System.Net;

namespace User.Api.Controllers
{


    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ApiControllerBase
    {
        #region services
        private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RedisService _redisService;
        public UserController(Project.Services.Interfaces.IAuthenticationService authenticationService, IHttpContextAccessor httpContextAccessor, RedisService redisService)
        {

            _redisService = redisService;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;

        }
        #endregion  
        [Route("UserRegister")]
        [HttpPost]
        public async Task<IActionResult> UserRegister(UserRegisterDto adduserDto)
        {
             
                var response = await _authenticationService.RegisterUser(adduserDto);
                if (response.StatusCode == 200 && !string.IsNullOrEmpty(response.Data.RefreshToken))
                {
                    SetRefreshTokenInCookie(response.Data.RefreshToken, (DateTime)response.Data.ExpiresOn);
                    return Ok(new { status = response.Succeeded, response.Message });
                }
                return StatusCode(response.StatusCode, new { status = response.Succeeded, response.Message, response.Errors });
        }
        [Route("CompanyRegister")]
        [HttpPost]
        public async Task<IActionResult> CompanyRegister(
         [FromForm] CompanyRegisterDto addCompanyDto)
        {
 //helllooo 
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             else 
             {
                 var response = await _authenticationService.RegisterCompany(addCompanyDto);
                 if (response.StatusCode == 200 && !string.IsNullOrEmpty(response.Data.RefreshToken))
                 {
                     SetRefreshTokenInCookie(response.Data.RefreshToken, (DateTime)response.Data.ExpiresOn);
                     return Ok(new { satue = response.Succeeded, response.Message });
                 }
                 return StatusCode(response.StatusCode, new { satue = response.Succeeded, response.Message, response.Errors });
             }
            
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> login(LoginDto request)
        {
           
                var response = await _authenticationService.Login(request);
                if (response.StatusCode == 200 && !string.IsNullOrEmpty(response.Data.RefreshToken))
                {
                    SetRefreshTokenInCookie(response.Data.RefreshToken, (DateTime)response.Data.ExpiresOn);
                    return Ok(new { status = response.Succeeded, response.Message, Data = new {token= response.Data.Token} });
                }
                return StatusCode(response.StatusCode, new { status = response.Succeeded, response.Message, response.Errors });
            
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var authorizationHeader = httpContext?.Request.Headers["Authorization"].ToString();
            string currentToken = authorizationHeader.Substring("Bearer ".Length);
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
   
                var response = await _authenticationService.Logout(refreshToken, currentToken);
                if (response)
                    return Ok();

                return BadRequest(new
                {
                    satus = "false",
                    massage = "failed refresh token"   
                });
            } 
            return BadRequest("no refresh token");
        }
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
         
                var response = await _authenticationService.ConfirmEmail(confirmEmailDto);
                if (response.Succeeded)
                    return Ok(new { status = response.Succeeded, response.Message, Data = new { token = response.Data.Token } });
                return StatusCode(response.StatusCode, new { status = response.Succeeded, response.Message, response.Errors });
            
        }
        [HttpPost("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword( [FromBody] [Required(ErrorMessage = "Password is required")]
                                                        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
                                                        string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else 
            {
                var result = await _authenticationService.ResetPassword(password);
                if (result.StatusCode == 200)
                    return Ok(new { satus = true, massage = "password is reseted" });
                return StatusCode(result.StatusCode, new { status = false, result.Message, result.Errors });
            }
        }

        [HttpGet("ResendOTP")]

        public async Task<IActionResult> ResendOTP(
            [Required(ErrorMessage = "Email is required"),]
            [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format")]
            string email)
        {
            bool result = await _authenticationService.ResendOTP(email);
            if (result == true)
                return Ok("code is sended");
            else return BadRequest("check your email");
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {

            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            { 
                var response = await _authenticationService.RefreshTokenAsync(refreshToken);
                if (response.StatusCode == 200)
                    return Ok(new { status = response.Succeeded, response.Message, Data = new {token= response.Data.Token} });

                return StatusCode(response.StatusCode, new
                {
                    satus = response.Succeeded,
                    response.Message,
                    response.Errors
                });
            }
            return BadRequest("there are no refresh token");
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)

        { 

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = false,
                IsEssential = true,
                SameSite = SameSiteMode.None  ,
                Path = "/"
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var response = await _authenticationService.ChangePasswordAsync(changePasswordDto);
                if (response.StatusCode == 200)
                    return Ok(new { status = response.Succeeded, response.Message });
                return StatusCode(response.StatusCode, new { status = response.Succeeded, response.Message, response.Errors });
            }
        } 











    }
}