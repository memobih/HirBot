using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Interfaces;

namespace User.Services.Implemntation
{
    public class EducationService : IEducationService
    { 
        private readonly IAuthenticationService service;
        private readonly UnitOfWork unitOfWork; 
        public EducationService(IAuthenticationService service, UnitOfWork unitOfWork)
        {
            this.service = service;
            this.unitOfWork = unitOfWork;
        }

        public async Task<APIOperationResponse<object>> AddEducationAsync(EducationDto education)
        {
            try
            { 
                var user=await service.GetCurrentUserAsync(); 
                Education  newEducation=new Education();
                newEducation.Privacy = education.privacy;
                newEducation.Start_Date=education.Start_Date;
                newEducation.InstituationName=education.InstituationName;
                newEducation.degree= education.degree;
                newEducation.End_Date=education.End_Date;
                newEducation.HighScool=education.HighScool;
                newEducation.FieldOfStudy=education.FieldOfStudy;
                newEducation.UserID = user.Id; 
               await unitOfWork._context.Educations.AddAsync(newEducation);
                await unitOfWork.SaveAsync();
            return APIOperationResponse<object>.Success(newEducation);

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured " , new List<string> { ex.Message});

            }

        }

        public async Task<APIOperationResponse<object>> GetAllEducationAsync()
        {
            var user = await service.GetCurrentUserAsync(); 
            var educations=unitOfWork._context.Educations.Where(e=>e.UserID==user.Id).ToList();
            return APIOperationResponse<object>.Success(new { education = educations } );
        }
    }
}
