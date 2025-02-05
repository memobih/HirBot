using HirBot.Comman.Idenitity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Project.Repository.Repository;
using User.Services.DataTransferObjects.Authencation;

namespace User.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ExternalAuthController : ApiControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;

        public ExternalAuthController(Project.Services.Interfaces.IAuthenticationService authenticationService ,SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _authenticationService = authenticationService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet("github-login")]
        public IActionResult GitHubLogin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/ExternalAuth/github-callback"
            }, "github");
        }

        [HttpGet("github-callback")]
        public async Task<IActionResult> GitHubCallback()
        {
            var result= await _authenticationService.GitHubCallback();

            if(result.StatusCode==200)
                return Redirect ( "https://external-site.com/targetpage?token=" +result.Data.Token);
            string url = "https://external-site.com/targetpag";
            return Redirect(url);
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = "/api/ExternalAuth/google-callback";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("google", redirectUrl);
            return Challenge(properties, "google");
        }
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {

            var result = await _authenticationService.GoogleCallback();
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (result.StatusCode == 200)
                return Ok( new { token = result.Data.Token }   );
           
            return BadRequest("login with google is failed");
        }

    }
}
