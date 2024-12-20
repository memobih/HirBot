using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using User.Services.DataTransferObjects.Authencation;

namespace Project.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<APIOperationResponse<object>> AddUser(UserRegisterDto userRegisterDto);
        //Task<APIOperationResponse<object>> AddCashierAsync(RegisterUserDto addCashierDto);
        //Task<APIOperationResponse<List<GetUserDto>>> GetAllAdminAsync();
        //Task<APIOperationResponse<List<GetUserDto>>> GetAllCashierAsync();
        //Task<APIOperationResponse<object>> ChangePasswordAsync(ChangePasswordDto changePasswordDto, string userId);
        Task<APIOperationResponse<object> > Login(LoginDto request);
        //Task<APIOperationResponse<GetUserDto>> GetUserByIdAsync(string userId);

        //Task<APIOperationResponse<object>> UpdateUserAsync(GetUserDto updateUserDto);
    }
}