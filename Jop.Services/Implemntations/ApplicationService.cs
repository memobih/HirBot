using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.Interfaces;
using Jop.Services.Responses;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Implemntations
{
    public class ApplicationService : IApplicationService
    {

        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork unitOfWork;
        public ApplicationService(IAuthenticationService authenticationService, UnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;
            this.unitOfWork = unitOfWork;
        }
        public async Task<APIOperationResponse<object>> ApplicateOnJob(int jobId)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var job = await unitOfWork.Jobs.GetLastOrDefaultAsync(j => j.ID == jobId);
                if (job == null || job.status != JobStatus.published)
                  return  APIOperationResponse<object>.BadRequest("You cannot apply on this job");
                var application = new Application
                {
                    UserID = user.Id,
                    JopID = jobId,
                    status = ApplicationStatus.pending

                };
               await unitOfWork.Applications.AddAsync(application);
                await unitOfWork.SaveAsync();
              return  APIOperationResponse<object>.Success("You are Applied Succefuly");

            }
            catch(Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there  are error accured ");
            }
        }
      
        public async  Task<APIOperationResponse<object>> GetALLAplications(int jobid , string? search = null, ApplicationStatus? status = null ,string columnsort="score",  string ?sort=null , int page=1 , int perpage=10)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null )
                    APIOperationResponse<object>.UnOthrized("this user is not a company");
                var job = await unitOfWork._context.Jobs.
                    Include(j => j.Applications)
                    .ThenInclude(a => a.User).ThenInclude(u=>u.Portfolio).FirstOrDefaultAsync(j=>j.ID==jobid);
                 if (job == null || company.ID!=job.CompanyID)
                 return   APIOperationResponse<object>.NotFound("this job is not found");
                var applications = new List< Applications>();
                if (job.Applications != null)
                {
                    foreach (var application in job.Applications)
                    {
                        if(application.User.Portfolio==null) application.User.Portfolio=new Portfolio();
                        applications.Add(
                            new Applications
                            { 
                                id= application.ID,
                                name = application.User.FullName,
                                email = application.User.Email,
                                Score = 80,
                                status = application.status,
                                created_at = application.CreationDate,
                                CVLink = application.User.Portfolio.CVUrl,
                                imageLink=application.User.ImagePath,
                                userName=application.User.UserName,
                            }

                          );
                    }
                }
                Filter(ref applications, jobid, search, status, sort, page, perpage);
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (applications.Count() / perpage) + 1, pageSize = perpage, totalRecords = applications.Count(), data = Paginate(applications, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there  are error accured ");
            }
        }
        public async Task<APIOperationResponse<object>> ApproveApplocations(List<int> ids)
        {
            try
            {
                 return await changeStatus(ids, ApplicationStatus.approved);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured"); 
            }
        }

        public async Task<APIOperationResponse<object>> RejectApplication(List<int> ids)
        {
            try
            {
                return await changeStatus(ids, ApplicationStatus.rejected);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        public async Task<APIOperationResponse<object>> DeleteApplications(List<int> ids)
        {
            try
            {

                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this email is not a company");
                var applications = await unitOfWork._context.Applications.Include(a => a.Job).Where(a => ids.Contains(a.ID) && a.Job.CompanyID == company.ID).ToListAsync();

       unitOfWork._context.Applications.RemoveRange(applications);
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the applications is updated succeful", "the applications is updated succeful  ");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");

            }
        }

        #region Helber  
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        private async Task<APIOperationResponse<object>>  changeStatus( List<int> ids , ApplicationStatus status)
        {
            try
            {

                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this email is not a company");
                var applications = await unitOfWork._context.Applications.Include(a=>a.Job).Where(a=>ids.Contains(a.ID)).ToListAsync();
                foreach (var application in applications)
                {
                    if (application.Job.CompanyID == company.ID)
                    {
                        application.status = status;
                    }
                }
                unitOfWork._context.Applications.UpdateRange(applications);
             await   unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the applications is updated succeful", "the applications is updated succeful  ");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");

            }
        }
     
        private void Filter(ref List<Applications> applications ,  int jobid, string? search = null, ApplicationStatus? status = null, string? sort = null, int page = 1, int perpage = 10)
        {
            if (search != null)
                applications = applications.Where(a =>
                a.email.StartsWith(search) ||
                a.name.StartsWith(search)

                ).ToList();
            if(status!=null)
            {
                applications = applications.Where(a => a.status == status).ToList();
            }
            applications.Reverse();
            if (sort != null)
            {

                if (sort != "asc")
                    applications = applications.OrderByDescending(j => j.Score).ToList();
                else applications = applications.OrderBy(j => j.Score).ToList();
            }
        }

     


        #endregion
    }
}
