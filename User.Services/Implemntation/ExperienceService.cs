﻿using HirBot.Comman.Enums;
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
using User.Services.Response;

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
                if(user==null) return APIOperationResponse<object>.NotFound("user is not found");
                var newExperience = new Experience();
                user = _unitOfWork._context.users.Include(u=>u.experiences).FirstOrDefault(u=>u.Id==user.Id); 
                newExperience.privacy = experience.privacy;
                newExperience.Start_Date = experience.startDate;
                newExperience.employeeType = experience.jobType;
                newExperience.End_Date = experience.endDate;
                newExperience.Title = experience.title;
                newExperience.location = experience.location;
                newExperience.workType = experience.workType;
                newExperience.companyName = experience.companyName;
                user.experiences?.Add(newExperience);
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
            var Experiences = await _unitOfWork._context.Experiences.Include(e=>e.Company).Where(e=>e.UserID==user.Id).ToListAsync(); 
            List<ExperienceResponse> ALL= new List<ExperienceResponse>();
            foreach (var experience in Experiences)
            {
                ExperienceResponse ExperiencesDetails = new ExperienceResponse();

                ExperiencesDetails.privacy = experience.privacy;
                ExperiencesDetails.startDate = experience.Start_Date;
                ExperiencesDetails.jobType = experience.employeeType;
                ExperiencesDetails.endDate = experience.End_Date;
                ExperiencesDetails.title = experience.Title;
                ExperiencesDetails.location = experience.location;
                ExperiencesDetails.workType=experience.workType;
                ExperiencesDetails.company = new ExpreienceCompany();

                if (experience.Company!=null)
                { 
                    var company= _unitOfWork._context.users.First(u=>u.CompanyID ==experience.CompanyID);
                    ExperiencesDetails.company.id= experience.CompanyID;
                    ExperiencesDetails.company.name = company.FullName; 
                    ExperiencesDetails.company.logo=experience.Company.Logo;
                }
                else
                {
                    ExperiencesDetails.company.name = experience.companyName;
                }
                ALL.Add(ExperiencesDetails);
            }

            return APIOperationResponse<object>.Success(new {Experiences = ALL });
        }
    }
}
