using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Authencation.ProfileDto;
using User.Services.Interfaces;

namespace User.Services.Implemntation
{
    public  class PersonalInformationService : IPersonalInformationService
    {  
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UnitOfWork _unitOfWork; 
        public PersonalInformationService(IAuthenticationService authenticationService , UserManager<ApplicationUser> userManager , UnitOfWork unitOfWork) {
            _authenticationService = authenticationService;
            _userManager = userManager;
            _unitOfWork = unitOfWork; 
        }

        public async Task<APIOperationResponse<PersonalInfoDto>> UpdatePersonalInformationAsync(PersonalInfoDto personalInfoDto)
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return APIOperationResponse<PersonalInfoDto>.UnOthrized();
            user = await _unitOfWork._context.users.Include(U => U.Portfolio).FirstOrDefaultAsync(u => u.Id == user.Id);
            if (user.Portfolio == null) user.Portfolio = new Portfolio();
            user.Portfolio.Title = personalInfoDto.Title;
            user.FullName = personalInfoDto.FullName;
            try
            {
                await _userManager.UpdateAsync(user);
                return APIOperationResponse<PersonalInfoDto>.Updated();

            }
            catch (Exception ex)
            {
                return APIOperationResponse<PersonalInfoDto>.BadRequest("ther are error accured ", ex.Message);
            }

        }
        public async Task<APIOperationResponse<PersonalInfoDto>> GetPersonalInformationAsync()
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null) return APIOperationResponse<PersonalInfoDto>.UnOthrized();
            user = await _unitOfWork._context.users.Include(U => U.Portfolio).FirstOrDefaultAsync(u => u.Id == user.Id);
            if (user.Portfolio == null) user.Portfolio = new Portfolio();
            PersonalInfoDto personalInfoDto = new PersonalInfoDto(); 
            personalInfoDto.Title=user.Portfolio.Title;
            personalInfoDto.FullName = user.FullName;
           return APIOperationResponse<PersonalInfoDto>.Success(personalInfoDto);
        }

       
    }
}
