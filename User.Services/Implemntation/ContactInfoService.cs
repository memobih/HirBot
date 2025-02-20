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
using HirBot.Comman.Enums;
using HirBot.Data.Entities;
namespace User.Services.Implemntation
{
    public class ContactInfoService  : IContactInfoService
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
           var user =  await  GetCurrentUserAsync() ;
            
            if(user == null || user.role != UserType.User)
            {
                return APIOperationResponse<ContactInfoDto>.UnOthrized("User not found");
            }
            
           var Portfolio =await  _unitOfWork._context.Portfolios.FirstOrDefaultAsync(p=> p.UserID==user.Id);
            if(Portfolio== null)
            {
                user.Portfolio = new HirBot.Data.Entities.Portfolio();
                await _userManager.UpdateAsync(user);  
                Portfolio=user.Portfolio;
            }
            ContactInfoDto contact = new ContactInfoDto();
            contact.Location = Portfolio.location;
            contact.PortfolioURL = Portfolio.PortfolioUrl;
            contact.ContactNumber = user.PhoneNumber;
            contact.GithubURL = Portfolio.GithubUrl; 
            return APIOperationResponse< ContactInfoDto > .Success(contact); 
        }

        public async Task< APIOperationResponse<ContactInfoDto> > UpdateContactInfoAsync(ContactInfoDto contactInfoDto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return APIOperationResponse<ContactInfoDto>.NotFound("User not found");
            }
            var portfolio = await _unitOfWork._context.Portfolios.FirstOrDefaultAsync(P=>P.UserID==user.Id);
            if (portfolio == null)
            {
                user.Portfolio = new HirBot.Data.Entities.Portfolio();
                await _userManager.UpdateAsync(user);
                portfolio = user.Portfolio;
            }
            portfolio.location = contactInfoDto.Location;
            portfolio.PortfolioUrl = contactInfoDto.PortfolioURL;
            user.PhoneNumber = contactInfoDto.ContactNumber;
            portfolio.GithubUrl = contactInfoDto.GithubURL; 
            await _unitOfWork.Users.UpdateAsync(user);
            _unitOfWork._context.Portfolios.Update(portfolio); 
            try
            {
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<ContactInfoDto>.Updated(message: "Contact info updated successfully");
            }
            catch (Exception e)
            {
                return APIOperationResponse<ContactInfoDto>.ServerError("Error while updating contact info", new List<string> { e.Message });
            }
        } 
            //}
            private async Task<ApplicationUser> GetCurrentUserAsync()
            {
                var currentUser = _contextAccessor.HttpContext.User;
                return await _userManager.GetUserAsync(currentUser);
            }
        }
    
}