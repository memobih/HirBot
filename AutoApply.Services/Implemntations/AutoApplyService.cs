using AutoApply.Services.Interfaces;
using HirBot.Comman.Enums;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.Data.IGenericRepository_IUOW;
using HirBot.ResponseHandler.Models;
using Jop.Services.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Prepare;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.Implemntation;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace AutoApply.Services.Implemntations
{
    public class AutoApplyService : IAutoApplyService
    { 
        private readonly UnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

         public AutoApplyService(UnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }

        public async Task<APIOperationResponse<object>> ChangeStatus(bool IsAuto)
        {
            try
            {
                var user=await _authenticationService.GetCurrentUserAsync();
                user.IsAutoApply = IsAuto;
               await _unitOfWork.Users.UpdateAsync(user);
              await  _unitOfWork.SaveAsync();

                return APIOperationResponse<object>.Success("Auto Apply is Changed", "Auto Apply is Changed");
                
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
          
        }

        public async Task<APIOperationResponse<object>> GetJobByAutoApply(string? search = null, string? experience = null, string? location = null, List<LocationType>? locationType = null, List<EmployeeType>? JobType = null, int page = 1, int perpage = 10, int? minSalary = null, int? maxSalary = null)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();

                var Applications = _unitOfWork._context.Applications.Include(J=>J.Job).ThenInclude(j => j.JobRequirments)
                    .ThenInclude(r => r.Skill).Include(j => j.Job).ThenInclude(j => j.JobRequirments).ThenInclude(r => r.Level).
                    Where(a => a.IsAuto ==true && a.UserID==user.Id).ToList();
                
                List<JobRecomendations> jobs = new List<JobRecomendations>();
                if (Applications != null)
                    foreach (var Application in Applications)
                    {
                      var  jop = Application.Job;

                        if (jop.status == JobStatus.published)
                        {


                            var added = new JobRecomendations
                            {
                                Description = jop.Description,
                                EmployeeType = jop.EmployeeType,
                                Experience = jop.Experience,
                                location = jop.location,
                                Salary = jop.Salary,
                                ID = jop.ID,
                                status = jop.status,
                                Title = jop.Title,
                                created_at = jop.CreationDate,
                                LocationType = jop.LocationType
                            };
                            if (jop.JobRequirments != null)
                            {
                                added.Skills = new List<Skills>();
                                foreach (var skill in jop.JobRequirments)
                                {
                                    added.Skills.Add(new Skills { name = skill.Skill.Name, evaluation = skill.Level.Name });
                                }
                            }
                            var company = _unitOfWork._context.Companies.Include(c => c.account).FirstOrDefault(c => c.ID == jop.CompanyID);
                            added.company.name = company.account.FullName;
                            added.company.logo = company.account.ImagePath;
                            var application = _unitOfWork._context.Applications.
                                Where(a =>
                            a.UserID == user.Id
                                  &&
                            a.JopID == jop.ID
                            ).FirstOrDefault();
                            if (application != null)
                                added.IsApplied = true;
                            else added.IsApplied = false;
                            jobs.Add(added);
                        }
                    }

                FilterRecomendationsJobs(ref jobs, search, experience, location, locationType, JobType, page, perpage, minSalary, maxSalary);
                return APIOperationResponse<object>.Success(new { remainingApplication=15-Applications?.Count()   , allApplication=15 ,  currentPage = page, totalPages = (jobs.Count() / perpage) + 1, pageSize = perpage, totalRecords = jobs.Count(), data = Paginate(jobs, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> GetStatus()
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();

                // عدد الطلبات التي تم تقديمها تلقائيًا
                var autoApplicationsCount = await _unitOfWork._context.Applications
                    .Where(a => a.IsAuto == true && a.UserID == user.Id)
                    .CountAsync();

                const int maxApplications = 15;

                var response = new
                {
                    isAutoApply = user.IsAutoApply,
                    remainingApplication = maxApplications - autoApplicationsCount,
                    allApplication = maxApplications
                };

                return APIOperationResponse<object>.Success(response);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("An error occurred while retrieving status.");
            }
        }

        #region Handler  
        private void FilterRecomendationsJobs(ref List<JobRecomendations> jobs, string? search = null, string? experience = null, string? location = null, List<LocationType>? locationType = null, List<EmployeeType>? JobType = null, int page = 1, int perpage = 10, int? minSalary = null, int? maxSalary = null)
        {
            if (search != null && !search.IsNullOrEmpty())
            {
                jobs = jobs.Where(
                    j => j.Title.StartsWith(search)
                    ||
                    j.Salary.ToString().StartsWith(search)
                    ||
                    j.location.ToString().StartsWith(search)
                    ).ToList();
            }
            if (experience != null)
                jobs = jobs.Where(j => j.Experience == experience).ToList();
            if (location != null)
                jobs = jobs.Where(j => j.location.StartsWith(location)).ToList();
            if (locationType != null)
                jobs = jobs.Where(j => locationType.Contains(j.LocationType)).ToList();
            if (JobType != null)
                jobs = jobs.Where(j => JobType.Contains(j.EmployeeType)).ToList();
            if (minSalary != null)
                jobs = jobs.Where(j => j.Salary >= minSalary).ToList();
            if (maxSalary != null)
                jobs = jobs.Where(j => j.Salary <= maxSalary).ToList();

        }
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        #endregion
    }
}
