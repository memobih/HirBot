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
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using User.Services.models;
using HirBot.Data.IGenericRepository_IUOW;
using Azure;
using HirBot.Redies;
using Newtonsoft.Json.Linq;

namespace User.Services.Implemntation
{
    public  class AuthenticationService :  Project.Services.Interfaces.IAuthenticationService
    {
        #region services
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RedisService _redisService;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration, RedisService redisService)
        {
            _userManager = userManager;
            _configuration = configuration; 
            _redisService = redisService;
        }
        #endregion
        #region Authenticated
        public async Task<APIOperationResponse<AuthModel>> RegisterUser(UserRegisterDto userRegisterDto)
        {
            var respon = new AuthModel();

            try
            { 
                var user =await _userManager.FindByEmailAsync(userRegisterDto.Email); 
                
                if(user!=null)
                {
                    return APIOperationResponse<AuthModel>.BadRequest("this email already register");
                } 

                var newUser = new ApplicationUser();
                newUser.Email = userRegisterDto.Email;
                newUser.FullName = userRegisterDto.FullName;
                newUser.PhoneNumber = userRegisterDto.PhoneNumber;
                newUser.UserName = userRegisterDto.Email;
                newUser.UserType = UserType.user;

                IdentityResult result = await _userManager.CreateAsync(newUser, userRegisterDto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return APIOperationResponse<AuthModel>.BadRequest(message: "Failed to register the  user. Please check the provided details.", errors);
                }
                string accessToken =await  GenerateJwtTokenAsync(newUser);
                RefreshToken refreshtoken = GenerateRefreshToken();
                respon.Email = newUser.Email;
                respon.RefreshToken = refreshtoken.token;
                respon.Token = accessToken;
                respon.Message = "created successful";
                respon.IsAuthenticated = true;
                respon.ExpiresOn = refreshtoken.expirationOn;
                newUser.refreshTokens?.Add(refreshtoken);
                await _userManager.UpdateAsync(newUser);
                return APIOperationResponse<AuthModel>.Success(respon, " user created successfully.");
            }
            catch (Exception ex)
            {
                respon.Message= "An error occurred while register the  user.";
                return APIOperationResponse<AuthModel>.ServerError(respon.Message, new List<string> { ex.Message });
            }
        } 
        public async Task<APIOperationResponse<AuthModel>> Login(LoginDto request)
        {
            var respon = new AuthModel();

            try
            { 
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    var isVaild = await _userManager.CheckPasswordAsync(user, request.Password);
                    if (isVaild)
                    {
                         string token = await GenerateJwtTokenAsync(user);
                        RefreshToken refreshtoken = GenerateRefreshToken();
                        respon.Email = user.Email;
                        respon.RefreshToken = refreshtoken.token;
                        respon.Token = token;
                        respon.Message = "login successful";
                        respon.IsAuthenticated = true;
                        respon.ExpiresOn = refreshtoken.expirationOn;
                        user.refreshTokens?.Add(refreshtoken);
                        await _userManager.UpdateAsync(user);
                        return APIOperationResponse<AuthModel>.Success(respon, message: "user is login");
                    }
                    return APIOperationResponse<AuthModel>.BadRequest("check your password or email");

                }
                return APIOperationResponse<AuthModel>.BadRequest("check your password or email");

            }
            catch (Exception ex)
            {
                return APIOperationResponse<AuthModel>.ServerError("An error occurred while register the  user.", new List<string> { ex.Message });
            }

        }  
        public async Task<bool> Logout(string token , string accessToken)
        {  

            var user= await _userManager.Users.FirstOrDefaultAsync(u=>u.refreshTokens.Any(t=>t.token==token));
            if(user==null)
               return false;
                      var refreshToken = user.refreshTokens.Single(t => t.token == token);
                      user.refreshTokens.Remove(refreshToken);
                      await _userManager.UpdateAsync(user);
                      var expiry = TimeSpan.FromDays(2);
                      var result = await _redisService.StoreJwtTokenAsync(accessToken, expiry);
            return true;

         
        }
        #endregion
        #region TOKEN AND REFRESH TOKEN
        private async Task<string > GenerateJwtTokenAsync(ApplicationUser user)
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
                expires: DateTime.Now.AddHours(2),
                signingCredentials: signingCredentials
                );

            string mytoken= new JwtSecurityTokenHandler().WriteToken(token);
           
            return mytoken;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

             var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                token = Convert.ToBase64String(randomNumber),
                expirationOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }
        public async Task<AuthModel> RefreshTokenAsync(string token)
        { 
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.refreshTokens.Any(t => t.token == token));
            var respon = new AuthModel();
            if (user == null)
            {
                respon.Message = "token is invalid";
                return respon;
            }

            var refreshToken = user.refreshTokens.Single(t => t.token == token); // the refreah token of the user

            if (!refreshToken.isActive)
            {
                respon.Message = "token is invalid";
            }

             string t = await GenerateJwtTokenAsync(user);
            return respon;
        }
        #endregion
    }
}
