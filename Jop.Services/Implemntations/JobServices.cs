using HirBot.Comman.Enums;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Project.Repository.Repository;
using Project.Services.Interfaces;
namespace Jop.Services.Implemntations
{
    public class JobServices : IJobService
    { 
        private readonly IAuthenticationService _authenticationService; 
        private readonly UnitOfWork unitOfWork;
        public JobServices ( IAuthenticationService authenticationService , UnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;
            this.unitOfWork = unitOfWork;
        }
        public async Task<APIOperationResponse<object>> AddJop(AddJobDto job)
        { 
            var user =await _authenticationService.GetCurrentUserAsync();
            if (user == null || user.role != UserType.Company) return APIOperationResponse<object>.UnOthrized(); 
             var company =user.Company ;
            if (company == null || company.status !=CompanyStatus.accepted) return APIOperationResponse<object>.BadRequest("this company is not accepted Yet");
            try
            {
                unitOfWork._context.Database.BeginTransaction();
                var newJob = new Job();
                #region mapping 
                newJob.Description = job.Description;
                newJob.Title = job.Title;
                newJob.LocationType = job.LocationType;
                newJob.EmployeeType = job.EmployeeType;
                newJob.CompanyID=company.ID;
                newJob.Experience = job.Experience; 
                newJob.status=job.status;
                newJob.Salary =job.Salary;
                #endregion 

                if (job.Requirments != null)
                    foreach (var requirment in job.Requirments)
                    {
                      newJob.JobRequirments?.Add(new JobRequirment {SkillID=requirment.SkillID , LevelID=requirment.LevelID , Rate=requirment.Rate });
                    }         

               await  unitOfWork.Jobs.AddAsync(newJob);
                await unitOfWork.SaveAsync();
                unitOfWork._context.Database.CommitTransaction();
                return APIOperationResponse<object>.Success("Jop is Created");
            }
            catch (Exception ex)
            {
                unitOfWork._context.Database.RollbackTransaction(); 
                return APIOperationResponse<object>.BadRequest("ther are error accured");
            }
          
            

    }

        public async Task<APIOperationResponse<object>> edit(int id, AddJobDto job)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null || user.role != UserType.Company) 
                return APIOperationResponse<object>.UnOthrized();
            var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c=>c.UserID==user.Id);
            if (company == null || company.status != CompanyStatus.accepted) 
                return APIOperationResponse<object>.BadRequest("this company is not accepted Yet");
          
                unitOfWork._context.Database.BeginTransaction();
                var editedJob = await unitOfWork.Jobs.GetEntityByPropertyWithIncludeAsync(j=>j.ID==id , j=>j.JobRequirments);
                if (editedJob.CompanyID!=company.ID)
                    return APIOperationResponse<object>.UnOthrized("Un outhorized to edited job");



                #region mapping 
                editedJob.Description = job.Description;
                editedJob.Title = job.Title;
                editedJob.LocationType = job.LocationType;
                editedJob.EmployeeType = job.EmployeeType;
                editedJob.CompanyID = company.ID;
                editedJob.Experience = job.Experience;
                editedJob.status = job.status;
                editedJob.Salary = job.Salary;
                #endregion


                if (editedJob.JobRequirments != null)
                {

                    unitOfWork._context.JobRequirements.RemoveRange(editedJob.JobRequirments);
                    editedJob.JobRequirments = null;
                }
                    
                if (job.Requirments != null)
                    foreach (var requirment in job.Requirments)
                    {
                        editedJob.JobRequirments?.Add(new JobRequirment { SkillID = requirment.SkillID, LevelID = requirment.LevelID, Rate = requirment.Rate });
                    }

                await unitOfWork.Jobs.UpdateAsync(editedJob);
                await unitOfWork.SaveAsync();
                unitOfWork._context.Database.CommitTransaction();
                return APIOperationResponse<object>.Success("Jop is Created");
            }
            catch (Exception ex)
            {
                unitOfWork._context.Database.RollbackTransaction();
                return APIOperationResponse<object>.BadRequest("ther are error accured");
            }
        }
    }
}
