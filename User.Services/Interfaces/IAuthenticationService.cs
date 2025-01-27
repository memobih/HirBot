using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using User.Services.DataTransferObjects.Authencation;
using User.Services.models;

namespace Project.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<APIOperationResponse<AuthModel>> RegisterUser(UserRegisterDto userRegisterDto);
        //Task<APIOperationResponse<object>> AddCashierAsync(RegisterUserDto addCashierDto);
        //Task<APIOperationResponse<List<GetUserDto>>> GetAllAdminAsync();
        //Task<APIOperationResponse<List<GetUserDto>>> GetAllCashierAsync();
        //Task<APIOperationResponse<object>> ChangePasswordAsync(ChangePasswordDto changePasswordDto, string userId);
        Task<APIOperationResponse<AuthModel> > Login(LoginDto request);
        Task<bool> Logout(string token ,string accesstoken);
        Task<APIOperationResponse<AuthModel>> RefreshTokenAsync(string token);
        public Task<APIOperationResponse<AuthModel>> RegisterCompany(CompanyRegisterDto companyRegisterDto);
        public Task<bool> ResendOTP(string email);
        public Task<APIOperationResponse<AuthModel>> ConfirmEmail(ConfirmEmailDto confirmemail);
        public Task<APIOperationResponse<object>> ResetPassword(PasswordDto password);
        public Task<APIOperationResponse<object>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        //Task<APIOperationResponse<GetUserDto>> GetUserByIdAsync(string userId);

        //Task<APIOperationResponse<object>> UpdateUserAsync(GetUserDto updateUserDto);
    }
}