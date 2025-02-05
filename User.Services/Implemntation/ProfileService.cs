using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using HirBot.Comman.Idenitity;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Project.Repository.Repository;
using User.Services.DataTransferObjects.Authencation.ProfileDto;
using User.Services.Interfaces;

namespace User.Services.Implemntation
{
    public class ProfileService : IProfileService
    {
       private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public ProfileService(IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager, IConfiguration configuration , UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<APIOperationResponse<PersonalInfoDto>> GetPersonalInfoAsync()
        {
            var user = GetCurrentUserAsync().Result;
            if (user == null)
            {
                  return APIOperationResponse<PersonalInfoDto>.NotFound(message: "User not found");
            }
            
            return null;
            
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var currentUser = _contextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }

     
    }
}