using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Jop.Services.Responses;
using Microsoft.EntityFrameworkCore;
using Notification.Services.Interfaces;
using Org.BouncyCastle.Asn1.Cms;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Interfaces;

namespace Jop.Services.Implemntations
{
    public class ApplicationService : IApplicationService
    {

        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork unitOfWork;
        private readonly IExperienceServices _experienceServices;
        private readonly INotificationService _notificationService;
        public ApplicationService(IAuthenticationService authenticationService, UnitOfWork unitOfWork, IExperienceServices experienceServices, INotificationService notificationService)
        {
            _experienceServices = experienceServices;
            _authenticationService = authenticationService;
            this.unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }
        public async Task<APIOperationResponse<object>> ApplicateOnJob(int jobId)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var job = await unitOfWork.Jobs.GetLastOrDefaultAsync(j => j.ID == jobId);
                if (job == null || job.status != JobStatus.published)
                    return APIOperationResponse<object>.BadRequest("You cannot apply on this job");
                var application = new Application
                {
                    UserID = user.Id,
                    JopID = jobId,
                    status = ApplicationStatus.pending,
                    CreationDate = DateTime.Now,

                };
                await unitOfWork.Applications.AddAsync(application);
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("You are Applied Succefuly");

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there  are error accured ");
            }
        }

        public async Task<APIOperationResponse<object>> GetALLAplications(int jobid, string? search = null, ApplicationStatus? status = null, string columnsort = "score", string? sort = null, int page = 1, int perpage = 10)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = unitOfWork._context.Companies.FirstOrDefault(c => c.UserID == user.Id);
                if (company == null)
                   return APIOperationResponse<object>.UnOthrized("this user is not a company");
                var job = await unitOfWork._context.Jobs.
                    Include(j => j.Applications)
                    .ThenInclude(a => a.User).ThenInclude(u => u.Portfolio).FirstOrDefaultAsync(j => j.ID == jobid);
                if (job == null || company.ID != job.CompanyID)
                    return APIOperationResponse<object>.NotFound("this job is not found");
                var applications = new List<Applications>();
                if (job.Applications != null)
                {
                    foreach (var application in job.Applications)
                    {
                        if (application.User.Portfolio == null) application.User.Portfolio = new Portfolio();
                        if (application.status != ApplicationStatus.approved)
                            applications.Add(
                                new Applications
                                {
                                    id = application.ID,
                                    name = application.User.FullName,
                                    email = application.User.Email,
                                    Score = 80,
                                    status = application.status,
                                    created_at = application.CreationDate,
                                    CVLink = application.User.Portfolio.CVUrl,
                                    imageLink = application.User.ImagePath,
                                    userName = application.User.UserName,
                                }

                              );
                    }
                    if (applications.Count == 0)
                        return APIOperationResponse<object>.NotFound("there are no applications");
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
                return await changeStatus(ids, ApplicationStatus.waiting);
               
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
                var company = unitOfWork._context.Companies.FirstOrDefault(c => c.UserID == user.Id);
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
        
        public async Task<APIOperationResponse<object>> GetAllApprovedApplications(int JobId,string? search = null, string columnsort = "score", string? sort = null, int page = 1, int perpage = 10)
        {
            try
            {
                var user = _authenticationService.GetCurrentUserAsync().Result;
                var company = unitOfWork._context.Companies.FirstOrDefault(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this email can not show this job ");
                var job = unitOfWork._context.Jobs.Include(j => j.Applications).ThenInclude(a=>a.Interviews).Include(a=>a.Applications).ThenInclude(a => a.User).ThenInclude(u => u.Portfolio).FirstOrDefault(j => j.ID == JobId);
                if (job == null || company.ID != job.CompanyID)
                    return APIOperationResponse<object>.NotFound("this job is not found");
                var applications = new List<ApprovedApplication>();
                if (job.Applications != null)
                {
                    foreach (var application in job.Applications)
                    {
                        if (application.User.Portfolio == null) application.User.Portfolio = new Portfolio();
                        if (application.status == ApplicationStatus.approved)
                            if (application.Interviews != null) application.Interviews=application.Interviews.OrderBy(a => a.CreationDate).ToList();
                            applications.Add(
                                new ApprovedApplication
                                {
                                    id = application.ID,
                                    name = application.User.FullName,
                                    email = application.User.Email,
                                    Score = 80,
                                    status = application.status,
                                    created_at = application.CreationDate,
                                    CVLink = application.User.Portfolio.CVUrl,
                                    imageLink = application.User.ImagePath,
                                    userName = application.User.UserName
                                    ,
                                    
                                    interviewType = application.Interviews?.LastOrDefault()?.Type ?? null
                                }

                              );
                    }
                    if (applications.Count == 0)
                        return APIOperationResponse<object>.NotFound("there are no applications aproved");
                }
                Filterapproved(ref applications, JobId, search, ApplicationStatus.approved, sort, page, perpage);
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (applications.Count() / perpage) + 1, pageSize = perpage, totalRecords = applications.Count(), data = Paginate(applications, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        public async Task<APIOperationResponse<AppuserDto>> GetApplicantDetails(int ApplicationId)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<AppuserDto>.UnOthrized("this email is not a company");
                var application = await unitOfWork._context.Applications.Include(a => a.User).ThenInclude(u => u.Portfolio).FirstOrDefaultAsync(a => a.ID == ApplicationId);
                if (application == null || application.status != ApplicationStatus.approved)
                    return APIOperationResponse<AppuserDto>.NotFound("this application is not found or not approved yet");
                var applicant = new AppuserDto
                {
                    Id = application.User.Id,
                    Name = application.User.FullName,
                    Email = application.User.Email,
                    jobTitle = application.User.Portfolio.Title,
                    image = application.User.ImagePath,
                    
                };
                return APIOperationResponse<AppuserDto>.Success(applicant, "the applicant details are fetched succefuly");

            }
            catch (Exception ex)
            {
                return APIOperationResponse<AppuserDto>.ServerError("there are error accured");

            }
        }

        
      
        #region Helber  
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        private async Task<APIOperationResponse<object>> changeStatus(List<int> ids, ApplicationStatus status)
        {
            try
            {

                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this email is not a company");
                var applications = await unitOfWork._context.Applications.Include(a => a.Job).Where(a => ids.Contains(a.ID)).ToListAsync();
                foreach (var application in applications)
                {
                    if (application.Job.CompanyID == company.ID)
                    {
                        application.status = status;
                        application.ModificationDate = DateTime.Now;
                        application.ModifiedBy = user.Id;
                    }
                }
                unitOfWork._context.Applications.UpdateRange(applications);
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the applications is updated succeful", "the applications is updated succeful  ");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");

            }
        }

        private void Filter(ref List<Applications> applications, int jobid, string? search = null, ApplicationStatus? status = null, string? sort = null, int page = 1, int perpage = 10)
        {
            if (search != null)
                applications = applications.Where(a =>
                a.email.StartsWith(search) ||
                a.name.StartsWith(search)

                ).ToList();
            if (status != null)
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
        private void Filterapproved(ref List<ApprovedApplication> applications, int jobid, string? search = null, ApplicationStatus? status = null, string? sort = null, int page = 1, int perpage = 10)
        {
            if (search != null)
                applications = applications.Where(a =>
                a.email.StartsWith(search) ||
                a.name.StartsWith(search)

                ).ToList();
            if (status != null)
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
        public async Task<APIOperationResponse<object>> AcceptTheApplication(int ApplicationId)
        {
            var application = await unitOfWork._context.Applications.Include(a => a.Job).ThenInclude(j => j.Company).FirstOrDefaultAsync(a => a.ID == ApplicationId);
            if (application == null)
                return APIOperationResponse<object>.NotFound("this application is not found");
            if (application.status != ApplicationStatus.approved)
                return APIOperationResponse<object>.BadRequest("this application is not approved yet");

                var user = await unitOfWork._context.Users.Include(c=>c.experiences).FirstOrDefaultAsync(u => u.Id == application.UserID);
                if (user == null)
                    return APIOperationResponse<object>.NotFound("this user is not found");
            user.experiences.Add(new Experience()
            {
                Title = application.Job.Title,
                companyName = application.Job.Company.Name,
                location = application.Job.location,
                Start_Date = DateTime.Now,
                 End_Date = null,
                workType = application.Job.LocationType,
                privacy = PrivacyEnum.Public,
                employeeType = application.Job.EmployeeType,
            });
            application.status = ApplicationStatus.accepted;
            try 
            {
                unitOfWork._context.Applications.Update(application);
                unitOfWork._context.Users.Update(user);
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the application is accepted succefuly", "the application is accepted succefuly");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured", new List<string> { ex.Message });
            }
        }

        public async Task<APIOperationResponse<object>> RejectTheApplication(int ApplicationId)
        {
            var application = await unitOfWork._context.Applications.Include(a => a.Job).ThenInclude(j => j.Company).FirstOrDefaultAsync(a => a.ID == ApplicationId);
            if (application == null)
                return APIOperationResponse<object>.NotFound("this application is not found");
            application.status = ApplicationStatus.rejected;
            try
            {
                unitOfWork._context.Applications.Update(application);
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the application is rejected succefuly", "the application is rejected succefuly");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured", new List<string> { ex.Message });
            }
        }
        public async Task<APIOperationResponse<object>> TrackApprovedApplications()
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            try
            {
                var applications = await unitOfWork._context.Applications
                    .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                    .Where(a => a.UserID == user.Id && a.status == ApplicationStatus.waiting)
                    .Select(a => new
                    {
                        id = a.ID,
                        jobTitle = a.Job.Title,
                        companyName = a.Job.Company.Name,
                        ApprovedTime = a.ModificationDate,
                        TimeAgo = DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalMinutes < 60 
                            ? $"{DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalMinutes:F0} minutes ago" 
                            : DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalHours < 24 
                                ? $"{DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalHours:F0} hours ago" 
                                : $"{DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalDays:F0} days ago",
                        Companyimage = a.Job.Company.Logo,
                    })
                    .ToListAsync();
                var interviews = await unitOfWork._context.Interviews
                    .Include(i => i.Application)
                    .Where(i => i.Application.UserID == user.Id 
                           && i.Application.status == ApplicationStatus.approved
                           && i.StartTime.Date >= DateTime.Now.Date)
                           .Select(i => new
                           {
                            id = i.ID,
                            jobTitle = i.Application.Job.Title,
                            companyName = i.Application.Job.Company.Name,
                            interviewDate = i.StartTime,
                            companyImage = i.Application.Job.Company.Logo,
                            status = i.Type,
                            TimeAgo = DateTime.Now.Subtract(i.CreationDate).TotalMinutes < 60 
                                ? $"{DateTime.Now.Subtract(i.CreationDate).TotalMinutes:F0} minutes ago" 
                                : DateTime.Now.Subtract(i.CreationDate).TotalHours < 24 
                                    ? $"{DateTime.Now.Subtract(i.CreationDate).TotalHours:F0} hours ago" 
                                    : $"{DateTime.Now.Subtract(i.CreationDate).TotalDays:F0} days ago"
                           })
                    .ToListAsync();
                if (!applications.Any() && !interviews.Any())
                  return APIOperationResponse<object>.NotFound("No Approved applications found or interviews found");

                return APIOperationResponse<object>.Success(new
                {
                    applications = applications,
                    interviews = interviews
                });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("An error occurred", new List<string> { ex.Message });
            }
        }
         public async Task<APIOperationResponse<object>> TrackRejectedApplications()
         {
            var user = await _authenticationService.GetCurrentUserAsync();
            try
            {
                var applications = await unitOfWork._context.Applications
                    .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                    .Where(a => a.UserID == user.Id && a.status == ApplicationStatus.rejected)
                    .Select(a => new
                    {
                        id = a.ID,
                        jobTitle = a.Job.Title,
                        companyName = a.Job.Company.Name,
                        Companyimage=a.Job.Company.Logo,
                        TimeAgo = DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalMinutes < 60 
                            ? $"{DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalMinutes:F0} minutes ago" 
                            : DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalHours < 24 
                                ? $"{DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalHours:F0} hours ago" 
                                : $"{DateTime.Now.Subtract(a.ModificationDate ?? DateTime.Now).TotalDays:F0} days ago",
                    })
                    .ToListAsync();

                if (!applications.Any())
                    return APIOperationResponse<object>.NotFound("No Rejected applications found");

                return APIOperationResponse<object>.Success(applications);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("An error occurred", new List<string> { ex.Message });

         }
               
        }
        public async Task<APIOperationResponse<object>> TrackInprogressApplications()
        {
            var user = _authenticationService.GetCurrentUserAsync().Result;
            try
            {
                var applications = unitOfWork._context.Applications
                    .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                    .Where(a => a.UserID == user.Id && a.status == ApplicationStatus.pending)
                    .Select(a => new
                    {
                        id = a.ID,
                        jobTitle = a.Job.Title,
                        companyName = a.Job.Company.Name,
                        appliedDateAgo = DateTime.Now.Subtract(a.CreationDate).TotalMinutes < 60 
                            ? $"{DateTime.Now.Subtract(a.CreationDate).TotalMinutes:F0} minutes ago" 
                            : DateTime.Now.Subtract(a.CreationDate).TotalHours < 24 
                                ? $"{DateTime.Now.Subtract(a.CreationDate).TotalHours:F0} hours ago" 
                                : $"{DateTime.Now.Subtract(a.CreationDate).TotalDays:F0} days ago",
                        Companyimage=a.Job.Company.Logo
                    })
                    .ToList();

                if (!applications.Any())
                    return APIOperationResponse<object>.NotFound("No Inprogress applications found");

                return APIOperationResponse<object>.Success(applications);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("An error occurred", new List<string> { ex.Message });
            }
        }
 public async Task<APIOperationResponse<object>> TrackAllApplications()
{
    var user = await _authenticationService.GetCurrentUserAsync();

    try
    {
        var allApplications = await unitOfWork._context.Applications
            .Include(a => a.Job)
                .ThenInclude(j => j.Company)
            .Include(a => a.Interviews)
            .Where(a => a.UserID == user.Id)
            .ToListAsync();

        if (!allApplications.Any())
            return APIOperationResponse<object>.NotFound("No applications found");

        var Applications = allApplications.Select<Application, object>(app =>
        {
            bool isApprovedWithInterview = app.status == ApplicationStatus.approved && app.Interviews.Any(i => i.StartTime > DateTime.Now);

            return isApprovedWithInterview 
                ? new
                {
                    Applicationid = app.ID,
                    status = "interview",
                    created_at = app.CreationDate,
                    message = "You have an upcoming interview.",
                    interview = app.Interviews.OrderByDescending(i => i.CreationDate).Take(1).Select(i => new
                    {
                        id = i.ID,
                        type = i.Type.ToString().ToLower(),
                        date = i.StartTime.ToString("yyyy-MM-dd"),
                        time = i.StartTime.ToString("hh:mm tt"),
                        duration = $"{i.durationInMinutes} minutes",
                        location = i.Mode == InterviewMode.Online ? "Zoom" : i.Location,
                        zoom_link = i.ZoomMeetinLink,
                        notes = i.Notes,
                        interviewername = i.InterviewerName,
                    }).ToList(),
                    job = new
                    {
                        id = app.Job.ID,
                        title = app.Job.Title,
                    },
                    company = new
                    {
                        id = app.Job.Company.ID,
                        name = app.Job.Company.Name,
                        logo = app.Job.Company.Logo,
                        websiteurl = app.Job.Company.websiteUrl,
                        location=new 
                        {
                            street=app.Job.Company.street,
                            country = app.Job.Company.country,
                            governorate = app.Job.Company.Governate,
                        }
                    }
                }
                : new
                {
                    id = app.ID,
                    status = app.status.ToString().ToLower(),
                    created_at = app.CreationDate,
                    job = new
                    {
                        id = app.Job.ID,
                        title = app.Job.Title,
                        location = app.Job.location,
                        salary = app.Job.Salary,
                        type = app.Job.LocationType.ToString()
                    },
                    company = new
                    {
                        id = app.Job.Company.ID,
                        name = app.Job.Company.Name,
                        logo = app.Job.Company.Logo
                    }               
                    
                 };
        }).ToList();

        return APIOperationResponse<object>.Success(new { applications = Applications });
    }
    catch (Exception ex)
    {
        return APIOperationResponse<object>.ServerError("An error occurred while fetching applications", new List<string> { ex.Message });
    }
}

        public async Task<APIOperationResponse<object>> StartProcess(int ApplicationId)
        {
            var user = await _authenticationService.GetCurrentUserAsync();

            var application = await unitOfWork._context.Applications.Include(a => a.Job).ThenInclude(j => j.Company).FirstOrDefaultAsync(a => a.ID == ApplicationId && a.UserID == user.Id);
            if (application == null)
                return APIOperationResponse<object>.NotFound("this application is not found or not yours");
            if(application.status== ApplicationStatus.approved)
                return APIOperationResponse<object>.BadRequest("this application is already approved");
            application.status = ApplicationStatus.approved;
            try
            {
                unitOfWork._context.Applications.Update(application);
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the Process has started succefuly", "the Process has started succefuly");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured", new List<string> { ex.Message });
            }
        }
        

        #endregion
    }
}
