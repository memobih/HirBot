using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Comman.Idenitity;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.Repository.Repository;
using Project.ResponseHandler.Consts;
using Project.ResponseHandler.Models;
using User.Services.DataTransferObjects;
using User.Services.Interfaces;

namespace User.Services.Implemntation
{
    public class ContactInfoService : IContactInfoService
    {
         private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public ContactInfoService(UserManager<ApplicationUser> userManager, IConfiguration configuration, UnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<APIOperationResponse<ContactInfoDto>> GetContactInfoAsync()
        {
            var user = GetCurrentUserAsync().Result ;
            if(user == null)
            {
                return APIOperationResponse<ContactInfoDto>.NotFound("User not found");
            }
            var ContactInfoDto=new ContactInfoDto();
            var portfoli=await _unitOfWork._context.Portfolios.FirstOrDefaultAsync(x=>x.UserID==user.Id);
            if(portfoli==null)
            {
                return APIOperationResponse<ContactInfoDto>.NotFound("Portfolio not found");
            }
            ContactInfoDto.Email=user.Email;
            ContactInfoDto.Location=portfoli.location;
            ContactInfoDto.PortfolioLink=portfoli.PortfolioUrl;
            ContactInfoDto.Profile=portfoli.PortfolioUrl;
            return APIOperationResponse<ContactInfoDto>.Success(ContactInfoDto);

        }

        public async Task<APIOperationResponse<ContactInfoDto>> UpdateContactInfoAsync(ContactInfoDto contactInfoDto)
        {
            var user = GetCurrentUserAsync().Result ;
            if(user == null)
            {
                return APIOperationResponse<ContactInfoDto>.NotFound("User not found");
            }
            var portfoli=await _unitOfWork._context.Portfolios.FindAsync(user.Id);
            if(portfoli==null)
            {
                return APIOperationResponse<ContactInfoDto>.NotFound("Portfolio not found");
            }
            portfoli.location=contactInfoDto.Location;
            portfoli.PortfolioUrl=contactInfoDto.PortfolioLink;
            user.Email=contactInfoDto.Email;
            _unitOfWork._context.Portfolios.Update(portfoli);
            _unitOfWork._context.Users.Update(user);
            try
            {
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<ContactInfoDto>.Updated(message:"Contact info updated successfully");
            }
            catch (Exception e)
            {
                return APIOperationResponse<ContactInfoDto>.ServerError("Error while updating contact info",new List<string>{e.Message});
            }
             
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var currentUser = _contextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }
    }
}