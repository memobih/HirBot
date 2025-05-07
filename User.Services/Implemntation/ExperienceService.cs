using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System.ComponentModel.Design;
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
                if (user == null) return APIOperationResponse<object>.NotFound("experiecne is not found");
                if (experience.CompanyID == null && experience.companyName == null)
                    return APIOperationResponse<object>.BadRequest("you must choose the name of the commpany ", new { companyName = "you must choose name " });
                var newExperience = new Experience();
                user = _unitOfWork._context.users.Include(u=>u.experiences).FirstOrDefault(u=>u.Id==user.Id);  
                
                newExperience.privacy = experience.privacy;
                newExperience.Start_Date = DateTime.Parse(experience.startDate);
                newExperience.employeeType = experience.jobType;
                newExperience.End_Date = DateTime.Parse(experience.endDate);
                newExperience.Title = experience.title;
                newExperience.location = experience.location;
                newExperience.workType = experience.workType;
                newExperience.CompanyID=experience.CompanyID;
                if (experience.CompanyID != null) 
                   experience.companyName =(await _unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.ID == experience.CompanyID )).Name;
                newExperience.companyName = experience.companyName;
                newExperience.CompanyID=experience.CompanyID;
                newExperience.UserID = user.Id;
                await _unitOfWork.Experiences.AddAsync(newExperience);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(experience);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured ",new List<string> { ex.Message } ); 
            }
        }

        public async Task<APIOperationResponse<object>> DeleteExperienceAsync(int id )
        {
            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                var experience = await _unitOfWork.Experiences.GetByIdAsync(id);
                if (user == null || experience == null || experience.UserID != user.Id)
                    return APIOperationResponse<object>.NotFound("user is not found"); 
                   await  _unitOfWork.Experiences.DeleteAsync(experience);
                  await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("Experience is delted");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured ", new List<string> { ex.Message });

            }

        }

        public async Task<APIOperationResponse<object>> EditExperienceAsync(int id, ExperienceDto experience)
        {


            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                var editedExperience = await _unitOfWork.Experiences.GetByIdAsync(id); 
                if (user == null  || editedExperience==null || editedExperience.UserID !=user.Id)  
                 return APIOperationResponse<object>.NotFound("experience is not found");     
                editedExperience.privacy = experience.privacy;
                editedExperience.Start_Date = DateTime.Parse(experience.startDate);
                editedExperience.employeeType = experience.jobType;
                editedExperience.End_Date = DateTime.Parse(experience.endDate);
                editedExperience.Title = experience.title;
                editedExperience.location = experience.location;
                editedExperience.workType = experience.workType;
                editedExperience.companyName = experience.companyName;
                await _unitOfWork.Experiences.UpdateAsync(editedExperience);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(experience);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured ", new List<string> { ex.Message });
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
                ExperiencesDetails.id = experience.ID;
                ExperiencesDetails.company = new ExpreienceCompany();
                if (experience.Company!=null)
                { 
                    var company = _unitOfWork._context.Companies.Include(C=>C.account).FirstOrDefault(c=>c.ID==experience.CompanyID);
                    ExperiencesDetails.company.id= experience.CompanyID;
                    ExperiencesDetails.company.name = company.account.FullName; 
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

        public async Task<APIOperationResponse<object>> GetExperienceAsyncByid(int id)
        {

            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                var experience = await _unitOfWork.Experiences.GetByIdAsync(id);
                if (user == null || experience == null || experience.UserID != user.Id)
                    return APIOperationResponse<object>.NotFound("experience is not found");
            
               ExperienceResponse ExperiencesDetails = new ExperienceResponse();
                ExperiencesDetails.privacy = experience.privacy;
                ExperiencesDetails.startDate = experience.Start_Date;
                ExperiencesDetails.jobType = experience.employeeType;
                ExperiencesDetails.endDate = experience.End_Date;
                ExperiencesDetails.title = experience.Title;
                ExperiencesDetails.location = experience.location;
                ExperiencesDetails.workType = experience.workType;
                ExperiencesDetails.id = experience.ID;
                ExperiencesDetails.company = new ExpreienceCompany();
                if (experience.Company != null)
                {
                    var company = _unitOfWork._context.Companies.Include(C => C.account).FirstOrDefault(c => c.ID == experience.CompanyID);
                    ExperiencesDetails.company.id = experience.CompanyID;
                    ExperiencesDetails.company.name = company.account.FullName;
                    ExperiencesDetails.company.logo = experience.Company.Logo;
                }
                else
                {
                    ExperiencesDetails.company.name = experience.companyName;
                }
                return APIOperationResponse<object>.Success(ExperiencesDetails);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured ", new List<string> { ex.Message });

            }
        }
    }
}
