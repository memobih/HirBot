using HirBot.Comman.Enums;
using HirBot.Comman.Idenitity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.Services.DataTransferObjects.Authencation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using User.Services.models;
using HirBot.Redies;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Project.Repository.Repository;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Mysqlx.Session;
using Mailing;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace User.Services.Implemntation
{
    public  class AuthenticationService :  Project.Services.Interfaces.IAuthenticationService
    {
        #region services
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RedisService _redisService;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthenticationService(IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager, IConfiguration configuration, RedisService redisService , UnitOfWork unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration; 
            _redisService = redisService;
        }
        #endregion
        #region Authenticated
        public async Task<APIOperationResponse<AuthModel>> RegisterUser(UserRegisterDto userRegisterDto)
        {

            try
            {
                var respon = new AuthModel();

                var user =await _userManager.FindByEmailAsync(userRegisterDto.Email); 
                
                if(user!=null)
                {
                    return APIOperationResponse<AuthModel>.Conflict("this email already register");
                } 

                var newUser = new ApplicationUser();
                newUser.Email = userRegisterDto.Email;
                newUser.FullName = userRegisterDto.FullName;
                //newUser.PhoneNumber = userRegisterDto.PhoneNumber;
                newUser.UserName = userRegisterDto.Email.Split('@')[0];
                newUser.role= UserType.User;

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
                respon.Username = newUser.UserName;
                respon.ExpiresOn = refreshtoken.expirationOn;
                respon.Role = newUser.role.ToString();
                respon.id = newUser.Id;
                var otp = GenerateOtp();
                newUser.VerificationCode = int.Parse(otp);   
                newUser.Code_Send_at=DateTime.UtcNow.AddMinutes(5);
                try
                {
                    SendEmail(newUser.Email, otp);
                 
                }
                catch (Exception ex)
                {
                    return APIOperationResponse<AuthModel>.ServerError("an error accured", new List<string> { ex.Message });
                } 
                newUser.refreshTokens?.Add(refreshtoken);
                await _userManager.UpdateAsync(newUser);
                return APIOperationResponse<AuthModel>.Success(respon, " user created successfully.");
            }
            catch (Exception ex)
            { 
                return APIOperationResponse<AuthModel>.UnprocessableEntity("an error accured", new List<string> { ex.Message });
            }
        }
        public async Task<APIOperationResponse<AuthModel>> ConfirmEmail(String Email ,int otp )
        {
            var respon = new AuthModel();

            var user =await _userManager.FindByEmailAsync(Email);
            if (user == null)
                return APIOperationResponse<AuthModel>.NotFound(message: "user not found");
            if (user.VerificationCode == otp && user.Code_Send_at >= DateTime.UtcNow )
            {
                string accessToken = await GenerateJwtTokenAsync(user);
                RefreshToken refreshtoken = GenerateRefreshToken();
                respon.Email = user.Email;
                respon.RefreshToken = refreshtoken.token;
                respon.Token = accessToken;
                respon.Username = user.UserName;
                respon.ExpiresOn = refreshtoken.expirationOn;
                respon.Role = user.role.ToString();
                respon.id = user.Id;
                user.IsVerified = true ;  
                await _userManager.UpdateAsync(user);
                return APIOperationResponse<AuthModel>.Success(respon, " user login successfully.");
            }
            return APIOperationResponse<AuthModel>.BadRequest(message: "un valid otp");
        }
        public async Task<APIOperationResponse<AuthModel>> RegisterCompany(CompanyRegisterDto companyRegisterDto)
        {

            try
            { 
                var respon = new AuthModel();

           

                var user = await _userManager.FindByEmailAsync(companyRegisterDto.CompanyEmail);

                if (user != null)
                {
                    return APIOperationResponse<AuthModel>.Conflict("this email already register");
                }

                var newUser = new ApplicationUser();
                newUser.Email = companyRegisterDto.CompanyEmail;
                newUser.FullName = companyRegisterDto.CompanyName;
                //newUser.PhoneNumber = userRegisterDto.PhoneNumber;
                newUser.UserName = companyRegisterDto.CompanyEmail;
                newUser.PhoneNumber = companyRegisterDto.ContactNumber;
               
                newUser.role = UserType.Company;

                IdentityResult result = await _userManager.CreateAsync(newUser, companyRegisterDto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return APIOperationResponse<AuthModel>.BadRequest(message: "Failed to register the  company. Please check the provided details.", errors);
                }
                string accessToken = await GenerateJwtTokenAsync(newUser);
                RefreshToken refreshtoken = GenerateRefreshToken();
                respon.Email = newUser.Email;
                respon.RefreshToken = refreshtoken.token;
                respon.Token = accessToken;
                respon.Username = newUser.UserName;
                respon.ExpiresOn = refreshtoken.expirationOn;
                respon.Role = newUser.role.ToString();
                respon.id = newUser.Id;
                newUser.refreshTokens?.Add(refreshtoken);

                await _userManager.UpdateAsync(newUser); 
                
                var newCompany = new Company();
                newCompany.Comments = companyRegisterDto.Comments;
                newCompany.status = CompanyStatus.process;
                newCompany.CreatedBy = newUser.Id;
                newCompany.ModifiedBy = newUser.Id;
                newCompany.country= companyRegisterDto.country;
                newCompany.street= companyRegisterDto.street;
                newCompany.Governate = companyRegisterDto.Governate;
                newCompany.websiteUrl = companyRegisterDto.websiteUrl;
                newCompany.SocialMeediaLink= companyRegisterDto.SocialMeediaLink;
                newCompany.CompanyType = companyRegisterDto.CompanyType;
                newCompany.TaxIndtefierNumber = companyRegisterDto.TaxIndtefierNumber;
                if (companyRegisterDto.BusinessLicense != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", companyRegisterDto.BusinessLicense.FileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await companyRegisterDto.BusinessLicense.CopyToAsync(stream);
                    }
                    newCompany.BusinessLicense = path;
                }
                await _unitOfWork.Companies.AddAsync(newCompany);
                return APIOperationResponse<AuthModel>.Success(respon, " company created successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<AuthModel>.UnprocessableEntity("an error accured", new List<string> { ex.Message });
            }
        }
        public async Task<APIOperationResponse<object>> ResetPassword(
            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        string password)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user== null)
                    return APIOperationResponse<object>.NotFound("user is not found");
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, password);
                return APIOperationResponse<object>.Success(new { masagee = "password reset successfuly" }, "password reset successfuly");

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.UnprocessableEntity("an error acuured" , new List<string> { ex.Message });
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var currentUser = _contextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }
        public async Task<APIOperationResponse<AuthModel>> Login(LoginDto request)
        {
            try
            {
                var respon = new AuthModel();

                var user = await _userManager.FindByEmailAsync(request.Email);
                
               
                if (user != null )
                {

                    if (user.IsVerified != true)
                        return APIOperationResponse<AuthModel>.UnOthrized("your email is not varified");

                    var isVaild = await _userManager.CheckPasswordAsync(user, request.Password);
                    if (isVaild)
                    {
                         string token = await GenerateJwtTokenAsync(user);
                        RefreshToken refreshtoken = GenerateRefreshToken();
                        respon.Email = user.Email;
                        respon.RefreshToken = refreshtoken.token;
                        respon.Token = token;
                        respon.ExpiresOn = refreshtoken.expirationOn;
                        respon.Username = user.UserName;
                        respon.Role = user.role.ToString();
                        respon.id = user.Id;
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
                return APIOperationResponse<AuthModel>.UnprocessableEntity("An error occurred while register the  user.", new List<string> { ex.Message });
            }

        }   
        public async Task<bool> ResendOTP(string email)
        {
            var user= await _userManager.FindByEmailAsync(email);
            if(user == null) 
                return false;
            var otp = GenerateOtp();
            user.Code_Send_at = DateTime.UtcNow.AddMinutes(5);
            user.VerificationCode = int.Parse (otp);
            await _userManager.UpdateAsync(user);
            SendEmail(email, otp);
            return true;

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
        #region Vlidate email 
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit OTP
        }
        private void SendEmail(string email, string otp)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background-color: #ffffff; padding: 40px; border-radius: 16px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
            <!-- Logo Section -->
            <div style='text-align: center; margin-bottom: 30px;'>
                <img src='YOUR_LOGO_URL' alt='HirBot Logo' style='max-width: 150px; height: auto;'>
            </div>

            <!-- Greeting -->
            <div style='text-align: center; margin-bottom: 30px;'>
                <h1 style='color: #6C71FF; margin: 0; font-size: 24px; font-weight: 700;'>Verify Your Account</h1>
                <p style='color: #666666; font-size: 16px; line-height: 24px; margin-top: 16px;'>
                    Please use the verification code below to complete your registration
                </p>
            </div>
            
            <!-- OTP Code Section -->
            <div style='text-align: center; margin: 40px 0;'>
                <div style='background: linear-gradient(145deg, #6C71FF 0%, #8A8DFF 100%); padding: 3px; border-radius: 12px;'>
                    <div style='background-color: white; padding: 20px; border-radius: 10px;'>
                        <div style='display: flex; justify-content: center; align-items: center; gap: 10px;'>
                            <span style='font-size: 32px; font-weight: bold; color: #6C71FF; letter-spacing: 8px;'>{otp}</span>
                            <button onclick='copyOTP()' style='background: none; border: none; cursor: pointer;'>
                                <img src='data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cGF0aCBkPSJNOCA0SDYgQzQuODk1NDMgNCA0IDQuODk1NDMgNCA2VjE4QzQgMTkuMTA0NiA0Ljg5NTQzIDIwIDYgMjBIMThDMTkuMTA0NiAyMCAyMCAxOS4xMDQ2IDIwIDE4VjE2TTggNEgxNkMxNy4xMDQ2IDQgMTggNC44OTU0MyAxOCA2VjE2TTggNEMxMy41MjI4IDQgMTggOC40NzcxNyAxOCAxNE0xOCAxNEMxOCAxNS42NTY5IDE2LjY1NjkgMTcgMTUgMTdIMTRDMTIuMzQzMSAxNyAxMSAxNS42NTY5IDExIDE0QzExIDEyLjM0MzEgMTIuMzQzMSAxMSAxNCAxMUgxNUMxNi42NTY5IDExIDE4IDEyLjM0MzEgMTggMTRaIiBzdHJva2U9IiM2QzcxRkYiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIiBzdHJva2UtbGluZWpvaW49InJvdW5kIi8+PC9zdmc+' 
                                     alt='Copy' style='width: 24px; height: 24px;'>
                            </button>
                        </div>
                    </div>
                </div>
                <p style='color: #666666; font-size: 14px; margin-top: 16px;'>
                    This code will expire in <span style='color: #6C71FF; font-weight: 600;'>5 minutes</span>
                </p>
            </div>

            <!-- Additional Info -->
            <div style='background-color: #f8f9ff; border-radius: 10px; padding: 20px; margin: 20px 0;'>
                <p style='color: #666666; font-size: 14px; line-height: 20px; margin: 0;'>
                    <span style='color: #6C71FF; font-weight: 600;'>Security Tip:</span> Never share this code with anyone. Our team will never ask for your verification code.
                </p>
            </div>
            
            <!-- Footer -->
            <div style='border-top: 1px solid #eeeeee; padding-top: 20px; text-align: center; margin-top: 30px;'>
                <p style='color: #999999; font-size: 12px;'>
                    If you didn't request this code, please ignore this email.
                </p>
                <div style='margin-top: 20px;'>
                    <a href='#' style='text-decoration: none; margin: 0 10px;'>
                        <img src='YOUR_FACEBOOK_ICON' alt='Facebook' style='width: 24px; height: 24px;'>
                    </a>
                    <a href='#' style='text-decoration: none; margin: 0 10px;'>
                        <img src='YOUR_TWITTER_ICON' alt='Twitter' style='width: 24px; height: 24px;'>
                    </a>
                    <a href='#' style='text-decoration: none; margin: 0 10px;'>
                        <img src='YOUR_LINKEDIN_ICON' alt='LinkedIn' style='width: 24px; height: 24px;'>
                    </a>
                </div>
                <p style='color: #999999; font-size: 12px; margin-top: 20px;'>
                    © {DateTime.Now.Year} HirBot. All rights reserved.
                </p>
            </div>
        </div>
    </div>

   
</body>
</html>";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HirBot", "hirbot.dev@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Your Verification Code";
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 465, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate("hirbot.dev@gmail.com", "xpspavohlgrxhcqd");
                    client.Send(message);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }
        #endregion
        #region TOKEN AND REFRESH TOKEN
        private async Task<string > GenerateJwtTokenAsync(ApplicationUser user)
        {  
            var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName! ),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
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
        public async Task<APIOperationResponse<AuthModel>> RefreshTokenAsync(string refreshtoken)
        {
           var token =HttpUtility.UrlDecode(refreshtoken);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.refreshTokens.Any(t => t.token == token));
            var respon = new AuthModel();
            if (user == null)
            {
                return APIOperationResponse<AuthModel>.BadRequest("invalid token");
            }
            var refreshToken = user.refreshTokens.Single(t => t.token == token); // the refreah token of the user hFgaaiAS1XpF+wdYOibMQJzCPAHUVwZ+EMEbLcGzLNs=

            if (!refreshToken.isActive)
            {
                return APIOperationResponse<AuthModel>.BadRequest("invalid token");
            }

            string mytoken = await GenerateJwtTokenAsync(user);
            respon.Email = user.Email;
            respon.RefreshToken = refreshToken.token;
            respon.Token = mytoken;
            respon.ExpiresOn = refreshToken.expirationOn;
            respon.Username = user.UserName;
            respon.Role = user.role.ToString();
            respon.id = user.Id;
            
            return APIOperationResponse<AuthModel>.Success(respon, message: "token is refreshed");
        }


        #endregion

        #region Password Change
   
        public async Task<APIOperationResponse<object>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            
            var user = await GetCurrentUserAsync();

            if (!await _userManager.CheckPasswordAsync(user, changePasswordDto.OldPassword))
            {
                return APIOperationResponse<object>.BadRequest("The old password is incorrect.");
            }
            try
            {
                
                if (user == null)
                    return APIOperationResponse<object>.NotFound("user is not found");
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
                if (result.Succeeded)
                    return APIOperationResponse<object>.Success(new { message = "password changed successfuly" }, "password changed successfuly");
                return APIOperationResponse<object>.BadRequest("check your password");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.UnprocessableEntity("an error accured", new List<string> { ex.Message });
            }
            
        }

        #endregion
    }
}
