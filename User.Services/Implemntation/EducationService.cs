using HirBot.Comman.Enums;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Microsoft.EntityFrameworkCore;
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
                newEducation.Type=education.Type;
                newEducation.FieldOfStudy=education.FieldOfStudy;
                newEducation.UserID = user.Id;
                newEducation.isGraduated = education.isGraduated;
                newEducation.logo = education.logo; 
               await unitOfWork._context.Educations.AddAsync(newEducation);
                await unitOfWork.SaveAsync();
            return APIOperationResponse<object>.Success(newEducation);

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured " , new List<string> { ex.Message});

            }

        }

        public async Task<APIOperationResponse<object>> DeleteEducationByIdAsync(int id)
        {

            try
            {
                var user = await service.GetCurrentUserAsync();
                var education = await unitOfWork.Educations.GetByIdAsync(id);
                if (education == null || education.UserID != user.Id)
                return APIOperationResponse<Object>.NotFound();

              await  unitOfWork.Educations.DeleteAsync(education);
                await unitOfWork.SaveAsync();

                return APIOperationResponse<object>.Success("education is deleted");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>>   EditEducationAsync(int id, EducationDto education)
        {
           try
            { 
                var user=await service.GetCurrentUserAsync();
                Education editedEducation =await  unitOfWork.Educations.GetByIdAsync(id);
                if (editedEducation==null  || editedEducation.UserID != user.Id)
                    return APIOperationResponse<object>.NotFound();
                editedEducation.Privacy = education.privacy;
                editedEducation.Start_Date=education.Start_Date;
                editedEducation.InstituationName=education.InstituationName;
                editedEducation.degree= education.degree;
                editedEducation.End_Date=education.End_Date;
                editedEducation.Type=education.Type;
                editedEducation.FieldOfStudy=education.FieldOfStudy;
                editedEducation.UserID = user.Id;
                editedEducation.isGraduated = education.isGraduated;
                editedEducation.logo = education.logo; 
                await unitOfWork.Educations.UpdateAsync(editedEducation);
                await unitOfWork.SaveAsync();
            return APIOperationResponse<object>.Success(editedEducation);

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
            return APIOperationResponse<object>.Success(new { educations = educations } );
        }

        public async Task<APIOperationResponse<object>> GetEducationByIdAsync(int id)
        {
            try
            {
                var user = await service.GetCurrentUserAsync();
                var education = await unitOfWork.Educations.GetByIdAsync(id);
                if(education !=null && education.UserID== user.Id)
                {
                    return APIOperationResponse<Object>.Success(education);
                }
                return APIOperationResponse<Object>.NotFound();

            }
            catch (Exception ex) {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
    }
}
