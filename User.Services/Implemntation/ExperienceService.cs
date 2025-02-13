using HirBot.Comman.Enums;
using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Interfaces;

namespace User.Services.Implemntation
{
    public class ExperienceService : IExperienceServices
    {
        private readonly IAuthenticationService authenticationService;
        private readonly UnitOfWork _unitOfWork;  
         public ExperienceService(IAuthenticationService authenticationService , UnitOfWork unitOfWork) { 
        this.authenticationService = authenticationService; 
        this._unitOfWork = unitOfWork;
        }

        public async Task<APIOperationResponse<object>> AddExperienceAsync(ExperienceDto experience)
        {
            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                var newExperience = new Experience();
                user = _unitOfWork._context.users.Include(u=>u.experiences).FirstOrDefault(u=>u.Id==user.Id); 
                newExperience.privacy = experience.privacy;
                newExperience.Start_Date = experience.Start_Date;
                newExperience.employeeType = experience.employeeType;
                newExperience.End_Date = experience.End_Date;
                newExperience.Title = experience.Title;
                newExperience.Location = experience.Location;
                user.experiences.Add(newExperience);
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(experience);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured ",new List<string> { ex.Message } ); 
            }
        }

        public async  Task<APIOperationResponse<object>> GetExperienceAsync()
        {
             var user = await authenticationService.GetCurrentUserAsync();
            var Experiences = await _unitOfWork._context.Experiences.Where(e=>e.UserID==user.Id).ToListAsync(); 
            return APIOperationResponse<object>.Success(new {Experience = Experiences});
        }
    }
}
