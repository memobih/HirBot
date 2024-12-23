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
using HirBot.Redies;
using Microsoft.AspNetCore.Http;


namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 

    public class UserController :ApiControllerBase
    {
        #region services
        private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RedisService _redisService;
        public UserController(Project.Services.Interfaces.IAuthenticationService authenticationService,IHttpContextAccessor httpContextAccessor, RedisService redisService)
        {
            
                _redisService= redisService;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;

        }
        #endregion  
        [Route("UserRegister")]
        [HttpPost]
        public async Task<IActionResult> UserRegister(UserRegisterDto adduserDto)
        {
            
            var response = await _authenticationService.RegisterUser(adduserDto);
            if(response.StatusCode==200&&!string.IsNullOrEmpty(response.Data.RefreshToken))
            {
                SetRefreshTokenInCookie(response.Data.RefreshToken, (DateTime)response.Data.ExpiresOn);
            }
            return StatusCode(response.StatusCode, response);
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> login(LoginDto request)
        {
            var response= await _authenticationService.Login(request);
            if (response.StatusCode == 200 && !string.IsNullOrEmpty(response.Data.RefreshToken))
            {
                SetRefreshTokenInCookie(response.Data.RefreshToken, (DateTime)response.Data.ExpiresOn);
            }
            return StatusCode(response.StatusCode, response);
        }
         [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(string token)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var authorizationHeader = httpContext?.Request.Headers["Authorization"].ToString();
            string currentToken= authorizationHeader.Substring("Bearer ".Length);
            var result = await _authenticationService.Logout(token, currentToken);
            if (result)
                return Ok();
           return BadRequest();
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate( string token)
        {

            if (await _redisService.IsTokenBlacklistedAsync(token))
            {
                return Unauthorized(new { Message = "Token is invalid or blacklisted." });
            }
            return Ok();
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}