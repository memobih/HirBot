using HirBot.Comman.Enums;
using HirBot.Comman.Idenitity;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Authencation;
using Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.Core;
using Project.ResponseHandler.Consts;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace User.Services.Implemntation
{
    public  class AuthenticationService :  Project.Services.Interfaces.IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration; 
        }
        public async Task<APIOperationResponse<object>> AddUser(UserRegisterDto userRegisterDto)
        {
            
            try
            {
                var newUser = new ApplicationUser();
                newUser.Email = userRegisterDto.Email;
                newUser.FullName = userRegisterDto.FullName;
                newUser.PhoneNumber = userRegisterDto.PhoneNumber;
                newUser.UserName = userRegisterDto.UserName;
                newUser.UserType = UserType.user;
                IdentityResult result = await _userManager.CreateAsync(newUser, userRegisterDto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return APIOperationResponse<object>.BadRequest(message: "Failed to register the  user. Please check the provided details.", errors);
                }
              var token = await GenerateJwtTokenAsync(newUser);
                return APIOperationResponse<object>.Success(new {result ,token}, " user created successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("An error occurred while register the  user.", new List<string> { ex.Message });
            }
        } 
        public async Task<APIOperationResponse<object>> Login(LoginDto request)
        {

            try
            {

                var user = await _userManager.FindByNameAsync(request.UserName);

                if (user != null)
                {
                    var isVaild = await _userManager.CheckPasswordAsync(user, request.Password);
                    if (isVaild)
                    {
                        var token = await GenerateJwtTokenAsync(user);
                        return APIOperationResponse<object>.Success(new { token }, message: "user is login");
                    }
                    return APIOperationResponse<object>.BadRequest("check your password or email");

                }
                return APIOperationResponse<object>.BadRequest("check your password or email");

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("An error occurred while register the  user.", new List<string> { ex.Message });
            }

        }


        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {  //Token claims
            var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName! ),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

      
    }
}
