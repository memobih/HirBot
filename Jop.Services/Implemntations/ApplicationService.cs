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
                    status = ApplicationStatus.waiting

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
      
        public async  Task<APIOperationResponse<object>> GetALLAplications(int jobid , string? search = null, ApplicationStatus? status = null , string ?sort=null , int page=1 , int perpage=10)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null)
                    APIOperationResponse<object>.UnOthrized("this user is not a company");
                var job = await unitOfWork._context.Jobs.
                    Include(j => j.Applications)
                    .ThenInclude(a => a.User).ThenInclude(u=>u.Portfolio).FirstOrDefaultAsync(j=>j.ID==jobid);
                 if (job == null)
                    APIOperationResponse<object>.NoContent("this job is not found");
                var applications = new List< Applications>();
                if (job.Applications != null)
                {
                    foreach (var application in job.Applications)
                    {
                        applications.Add(
                            new Applications
                            {
                                name = application.User.FullName,
                                email = application.User.Email,
                                Score = 80,
                                status = application.status,
                                created_at = application.CreationDate,
                                CVLink = application.User.Portfolio.CVUrl
                            }

                          );
                    }
                }
                Filter(ref applications, jobid, search, status, sort, page, perpage);
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (applications.Count() / perpage) + 1, pageSize = perpage, totalRecords = applications.Count(), applications = Paginate(applications, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there  are error accured ");
            }
        }
        #region Helber  
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
