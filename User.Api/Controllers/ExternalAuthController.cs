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

namespace User.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ExternalAuthController : ApiControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalAuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
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
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
                return Unauthorized(new { message = "GitHub login failed" });

            var claims = result.Principal.Claims.ToDictionary(c => c.Type, c => c.Value);

            var userId = claims[ClaimTypes.NameIdentifier];
            var username = claims[ClaimTypes.Name];
            var email = claims.TryGetValue(ClaimTypes.Email, out var emailClaim) ? emailClaim : null;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email
                };
                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded)
                    return BadRequest(identityResult.Errors);
            }
            return Ok(user);
        }

    }
}
