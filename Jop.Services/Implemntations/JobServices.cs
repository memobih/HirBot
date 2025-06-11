using HirBot.Comman.Enums;
using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.Data.IGenericRepository_IUOW;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Jop.Services.Helpers;
using Jop.Services.Interfaces;
using Jop.Services.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using Notification.Services.Interfaces;
using Project.Repository.Repository;
using Project.Services.Interfaces;
namespace Jop.Services.Implemntations
{
    public class JobServices : IJobService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork unitOfWork;
        private readonly INotificationService _notificationService;
        public JobServices(IAuthenticationService authenticationService, UnitOfWork unitOfWork, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _authenticationService = authenticationService;
            this.unitOfWork = unitOfWork;
        }
        public async Task<APIOperationResponse<object>> AddJop(AddJobDto job)
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null || user.role != UserType.Company) return APIOperationResponse<object>.UnOthrized();
            var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
            if (company == null || company.status != CompanyStatus.accepted) return APIOperationResponse<object>.BadRequest("this company is not accepted Yet");
            try
            {
                unitOfWork._context.Database.BeginTransaction();
                var newJob = new HirBot.Data.Entities.Job();
                #region mapping 
                newJob.Description = job.Description;
                newJob.Title = job.Title;
                newJob.LocationType = job.LocationType;
                newJob.EmployeeType = job.EmployeeType;
                newJob.CompanyID = company.ID;
                newJob.Experience = job.Experience;
                newJob.status = job.status;
                newJob.Salary = job.Salary;
                newJob.location = job.location;
                #endregion 

                if (job.Requirments != null)
                {
                    newJob.JobRequirments = new List<JobRequirment>();

                    foreach (var requirment in job.Requirments)
                    {

                        var skill = unitOfWork._context.Skills.FirstOrDefault(s => s.ID == requirment.SkillID);

                        var level = unitOfWork._context.Levels.FirstOrDefault(l => l.ID == requirment.LevelID);
                        if (skill == null || level == null)
                        {
                            return APIOperationResponse<object>.NotFound("the level or skill is not found");
                        }
                        newJob.JobRequirments?.Add(new JobRequirment { SkillID = requirment.SkillID, LevelID = requirment.LevelID });
                    }
                }

                await unitOfWork.Jobs.AddAsync(newJob);
                await unitOfWork.SaveAsync();
                unitOfWork._context.Database.CommitTransaction();
                var users=await unitOfWork._context.Users.Include(i=>i.Portfolio).Where(u => u.Portfolio.Title==job.Title).ToListAsync();
                if (users != null && users.Count > 0)
                {
                    List<string> recievers = new List<string>();
                    foreach (var userw in users)
                    {
                        recievers.Add(userw.Id);
                    }
                    await _notificationService.SendNotificationAsync($"New Job is added with title {job.Title}", NotificationType.job,NotficationStatus.created, newJob.ID.ToString(), recievers,
                    new
                    {
                        message = $"New Job is added with title {job.Title}",
                        Created_at = DateTime.Now,
                        type = new
                        {
                            action = "Created",
                            category = "job",
                            label = "New Job"
                        },
                        metadata = new
                        {
                            job = new
                            {
                                id = newJob.ID,
                                PostedDate = DateTime.Now,
                                CompanyLogo = company.Logo,
                                CompanyName = company.Name,

                            },
                            company = new
                            {
                                id = company.ID,
                                name = company.Name?? "",
                                logo = company.Logo?? "",
                            }
                        }
                    }
                    );
                }
                return APIOperationResponse<object>.Success("Jop is Created", "Jop is Created Succesful !");
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
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.BadRequest("this company is not accepted Yet");

                unitOfWork._context.Database.BeginTransaction();
                var editedJob = await unitOfWork.Jobs.GetEntityByPropertyWithIncludeAsync(j => j.ID == id, j => j.JobRequirments);
                if (editedJob.CompanyID != company.ID)
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
                editedJob.location = job.location;
                #endregion


                if (editedJob.JobRequirments != null)
                {

                    unitOfWork._context.JobRequirements.RemoveRange(editedJob.JobRequirments);
                    editedJob.JobRequirments = null;
                }

                if (job.Requirments != null)
                {
                    editedJob.JobRequirments = new List<JobRequirment>();

                    foreach (var requirment in job.Requirments)
                    {
                        var skill = unitOfWork._context.Skills.FirstOrDefault(s => s.ID == requirment.SkillID);

                        var level = unitOfWork._context.Levels.FirstOrDefault(l => l.ID == requirment.LevelID);
                        if (skill == null || level == null)
                        {
                            return APIOperationResponse<object>.NotFound("the level or skill is not found");
                        }
                        editedJob.JobRequirments?.Add(new JobRequirment { SkillID = requirment.SkillID, LevelID = requirment.LevelID });
                    }
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

        public async Task<APIOperationResponse<object>> getAllJobs(string? search = null, JobStatus? status = null, LocationType? locationType = null, EmployeeType? JobType = null, int page = 1, int perpage = 10, string? sort = "salary", string sortDirection = null)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company)
                    return APIOperationResponse<object>.UnOthrized("this user is not a company");
                var company = await unitOfWork._context.Companies.Include(C => C.jobs).
                     ThenInclude(j => j.JobRequirments).
                     ThenInclude(r => r.Skill)
                     .Include(c => c.jobs)
                     .ThenInclude(j => j.JobRequirments)
                     .ThenInclude(r => r.Level).OrderByDescending(j => j.CreationDate).Include(C => C.jobs).ThenInclude(j => j.Applications).
                     FirstOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this company is not accepted yet");
                List<JobListResponse> jobs = new List<JobListResponse>();
                if (company.jobs != null)
                    foreach (var jop in company.jobs)
                    {
                        if (jop.status != JobStatus.drafted)
                        {
                            var added = new JobListResponse
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
                                    added.Skills.Add(new Skills { SkillID=skill.SkillID , levelID=skill.LevelID,name = skill.Skill.Name, evaluation = skill.Level.Name });
                                }
                            }
                            added.ApplicantNumber = jop.Applications.Count();
                            jobs.Add(added);
                        }
                    }
                Filter(ref jobs, search, status, locationType, JobType, page, perpage, sort, sortDirection);

                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (jobs.Count() / perpage) + 1, pageSize = perpage, totalRecords = jobs.Count(), data = Paginate(jobs, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("THER ARE ERROR ACCURED");
            }

        }
        public async Task<APIOperationResponse<object>> getAllDraftedJobs(string? search = null, JobStatus? status = null, LocationType? locationType = null, EmployeeType? JobType = null, int page = 1, int perpage = 10, string? sort = "salary", string sortDirection = null)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company)
                    return APIOperationResponse<object>.UnOthrized("this user is not a company");
                var company = await unitOfWork._context.Companies.Include(C => C.jobs).
                    ThenInclude(j => j.JobRequirments).
                    ThenInclude(r => r.Skill)
                    .Include(c => c.jobs)
                    .ThenInclude(j => j.JobRequirments)
                    .ThenInclude(r => r.Level).OrderByDescending(j => j.CreationDate).
                    FirstOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this company is not accepted yet");
                List<JobListResponse> jobs = new List<JobListResponse>();
                if (company.jobs != null)
                    foreach (var jop in company.jobs)
                    {
                        if (jop.status == JobStatus.drafted)
                        {
                            var added = new JobListResponse
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
                                    added.Skills.Add(new Skills { SkillID = skill.SkillID, levelID = skill.LevelID, name = skill.Skill.Name, evaluation = skill.Level.Name });
                                }
                            }
                            jobs.Add(added);
                        }
                    }


                Filter(ref jobs, search, status, locationType, JobType, page, perpage, sort, sortDirection);
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (jobs.Count() / perpage) + 1, pageSize = perpage, totalRecords = jobs.Count(), data = Paginate(jobs, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("THER ARE ERROR ACCURED");
            }

        }
        public async Task<APIOperationResponse<object>> JobDetails(int id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company =  unitOfWork._context.Companies.FirstOrDefault(c => c.UserID == user.Id);
                var job = await unitOfWork._context.Jobs
                    .Include(j => j.JobRequirments).
                    ThenInclude(r => r.Level).
                    Include(j => j.JobRequirments).ThenInclude(r => r.Skill).OrderByDescending(j=>j.CreationDate).FirstOrDefaultAsync(j => j.ID == id);
                if (job == null)
                    return APIOperationResponse<object>.NotFound("this job is not found");
                if (user == null || job == null ||
                    (
                    (job.status == JobStatus.drafted || job.status == JobStatus.closed)
                    &&
                    (company == null || job.CompanyID != company.ID)
                    )
                    )
                    return APIOperationResponse<object>.UnOthrized("this user is un outhorized to show this job");

                JobDetailsResponse response = new JobDetailsResponse();
                response.Title = job.Title;
                response.Description = job.Description;
                response.location = job.location;
                response.status = job.status;

                response.LocationType = job.LocationType;
                response.EmployeeType = job.EmployeeType;
                response.Experience = job.Experience;
                response.Salary = job.Salary;
                response.ID = job.ID;
                
                var JobCompany = await unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.ID == job.CompanyID,c=>c.account );
                response.Company.name = JobCompany.account.FullName;
                response.Company.logo = JobCompany.account.ImagePath;
                response.Company.CompanyType = JobCompany.CompanyType;
                response.Company.location = JobCompany.street + "/" + JobCompany.Governate + "/" + JobCompany.country;
                var application = unitOfWork._context.Applications.FirstOrDefault(a => a.UserID == user.Id && a.JopID == job.ID);
                if (application!=null)
                    response.IsApplied = true;
                else response.IsApplied = false;
               
                
                if (job.JobRequirments != null)
                {
                    response.requiremnts = new List<Requiremnts>();
                    foreach (var requirment in job.JobRequirments)
                    {
                        response.requiremnts.Add(new Requiremnts { skillID=requirment.Skill.ID , levelID=requirment.LevelID,Skill = requirment.Skill.Name, level = requirment.Level.Name });
                    }
                }
                return APIOperationResponse<object>.Success(response);
            }
            catch (Exception ex) {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        public async Task<APIOperationResponse<object>> JobDetailsCompany(int id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = unitOfWork._context.Companies.FirstOrDefault(c => c.UserID == user.Id);
                var job = await unitOfWork._context.Jobs
                    .Include(j => j.JobRequirments).
                    ThenInclude(r => r.Level).
                    Include(j => j.JobRequirments).ThenInclude(r => r.Skill).OrderByDescending(j => j.CreationDate).Include(j=>j.Applications).ThenInclude(a=>a.User).FirstOrDefaultAsync(j => j.ID == id);
                if (job == null)
                    return APIOperationResponse<object>.NotFound("this job is not found");
                if (user == null || job == null ||
                    (
                    (job.status == JobStatus.drafted || job.status == JobStatus.closed)
                    &&
                    (company == null || job.CompanyID != company.ID)
                    )
                    )
                    return APIOperationResponse<object>.UnOthrized("this user is un outhorized to show this job");

                JobDetailsResponse response = new JobDetailsResponse();
                response.Title = job.Title;
                response.Description = job.Description;
                response.location = job.location;
                response.status = job.status;

                response.LocationType = job.LocationType;
                response.EmployeeType = job.EmployeeType;
                response.Experience = job.Experience;
                response.Salary = job.Salary;
                response.ID = job.ID;
                
                var JobCompany = await unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.ID == job.CompanyID, c => c.account);
                response.Company.name = JobCompany.account.FullName;
                response.Company.logo = JobCompany.account.ImagePath;
              List<Applications> applications = new List<Applications>();
                if (job.JobRequirments != null)
                {
                    response.requiremnts = new List<Requiremnts>();
                    foreach (var requirment in job.JobRequirments)
                    {
                        response.requiremnts.Add(new Requiremnts { skillID = requirment.Skill.ID, levelID = requirment.LevelID, Skill = requirment.Skill.Name, level = requirment.Level.Name });
                    }
                }
                if (job.Applications != null)
                {
                      
                    foreach (var application in job.Applications)
                    {
                       
                        var portofolio=unitOfWork._context.Portfolios.FirstOrDefault(p=>p.UserID==application.UserID);
                        var experiences=unitOfWork._context.Experiences.Where(e=>e.UserID==application.UserID).ToList();
                        application.User.Portfolio=portofolio;
                        application.User.experiences=experiences;
                        var skills = unitOfWork._context.UserSkills.Include(s => s.Skill).Where(e => e.UserID == application.UserID).Select(s => s.Skill.Name).ToList();
                        applications.Add(
                            new Applications
                            {
                                id = application.ID
                            ,
                                name = application.User.FullName,
                                email = application.User.Email,
                                Score = JobMatcher.GetScore(user, job, skills)
                            ,
                                status = application.status,
                                CVLink = portofolio?.CVUrl,
                                created_at = application.CreationDate,
                                imageLink = application.User.ImagePath,
                                userName = application.User.UserName
                            }

                          );
                    }

    }
                sortApplication(ref applications);

                return APIOperationResponse<object>.Success(new {details =response , topApplications=new { numberOfApplications = job.Applications?.Count() ?? 0, applications } });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> DeleteJobs(List<int> ids)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                var jobs = unitOfWork._context.Jobs.Where(j => ids.Contains(j.ID)).Where(j => j.CompanyID == company.ID).ToList();
                unitOfWork._context.Jobs.RemoveRange(jobs);
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("the jobs deleted succefuly");
            }
            catch (Exception e)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured ");
            }
        }

        public async Task<APIOperationResponse<object>> GetJosRecomendations(string? search = null, string? experience = null, string? location = null, List<LocationType>? locationType = null, List<EmployeeType>? JobType = null, int page = 1, int perpage = 10, int? minSalary = null, int? maxSalary = null)
        {
            try
            {
                var all = unitOfWork._context.Jobs.Include(j => j.JobRequirments)
                    .ThenInclude(r => r.Skill).Include(j => j.JobRequirments).ThenInclude(r => r.Level).
                    Where(j => j.status == JobStatus.published).ToList();
                List<JobRecomendations> jobs = new List<JobRecomendations>();
                var user = await _authenticationService.GetCurrentUserAsync();
                user = unitOfWork._context.users.Include(u => u.Skills).Include(u => u.Portfolio).Where(u => u.Id == user.Id).First();
                if (all != null)
                    foreach (var jop in all)
                    {
                        if (jop.status == JobStatus.published)
                        {
                            if (user.Portfolio == null) user.Portfolio = new Portfolio();
                            var candidate = GetUserInformations(user);
                            var ids = candidate.Skills.Select(s => s.SkillID).ToList();
                            var skills = unitOfWork._context.Skills.Where(s => ids.Contains(s.ID)).Select(s => s.Name).ToList();

                            var added = new JobRecomendations
                            {
                                Description = jop.Description,
                                EmployeeType = jop.EmployeeType,
                                Experience = jop.Experience,
                                location = jop.location,
                                Salary = jop.Salary,
                                ID = jop.ID,
                                status = jop.status,
                                score= JobMatcher.GetScore(candidate, jop, skills),
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
                            var company = unitOfWork._context.Companies.Include(c=>c.account).FirstOrDefault(c=>c.ID==jop.CompanyID);
                            added.company.name = company.account.FullName;
                            added.company.logo = company.account.ImagePath;
                            var application =  unitOfWork._context.Applications.
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
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (jobs.Count() / perpage) + 1, pageSize = perpage, totalRecords = jobs.Count(), data = Paginate(jobs, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured");
            }
        }
        public async Task<APIOperationResponse<object>> EditJobStatus(int id, EditStatusDto status)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                var job = await unitOfWork.Jobs.GetLastOrDefaultAsync(j => j.ID == id);
                if (company == null || company.status != CompanyStatus.accepted || company.ID != job.CompanyID)
                    return APIOperationResponse<object>.UnOthrized("this user is not uthorized to edit this job");
                job.status = status.status;
                await unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("status updated successful ", "status updated successful ");
            }
            catch (Exception ex) {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }


        #region Helper 
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        private void sortApplication(ref  List< Applications>  Applications )
        {
          
                Applications=Applications.OrderByDescending(a=>a.created_at).ToList();
                Applications = Paginate(Applications, 1, 10);
            Applications = Applications.OrderByDescending(a=>a.Score).ToList();
        }
        private void FilterRecomendationsJobs(ref List<JobRecomendations> jobs ,string? search = null, string? experience = null, string? location = null, List<LocationType>? locationType = null, List<EmployeeType>? JobType = null, int page = 1, int perpage = 10, int? minSalary = null, int? maxSalary = null )
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
                 if(experience!=null)
                jobs=jobs.Where(j=>j.Experience==experience).ToList();
            if (location != null)
                jobs = jobs.Where(j => j.location.StartsWith(location)).ToList();
            if (locationType != null)
                jobs = jobs.Where(j => locationType.Contains(j.LocationType)).ToList();
            if (JobType != null)
                jobs = jobs.Where(j => JobType.Contains(j.EmployeeType)).ToList();
            if(minSalary !=null)
                jobs = jobs.Where(j =>j.Salary >= minSalary).ToList();
            if(maxSalary != null)
                jobs = jobs.Where(j => j.Salary <= maxSalary).ToList();

        }


        private void Filter(ref List<JobListResponse> jobs , string? search = null, JobStatus? status = null, LocationType? locationType = null, EmployeeType? JobType = null, int page = 1, int perpage = 10 , string? sort = "salary", string sortDirection = null)
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
            if (status != null )
                jobs = jobs.Where(j => j.status == status).ToList();
            if (locationType != null)
                jobs = jobs.Where(j => j.LocationType == locationType).ToList();
            if (JobType != null)
                jobs = jobs.Where(j => j.EmployeeType == JobType).ToList();
            jobs.Reverse();
            if (sortDirection!=null)
            {
                if (sortDirection != "asc")
                    jobs = jobs.OrderByDescending(j => j.Salary).ToList();
                else jobs = jobs.OrderBy(j => j.Salary).ToList();
            }
          
        }

        private ApplicationUser GetUserInformations(ApplicationUser user)
        {
            user.Skills = unitOfWork._context.UserSkills.Where(s => s.UserID == user.Id).ToList();
            user.experiences = unitOfWork._context.Experiences.Where(e => e.UserID == user.Id).ToList();
            return user;
        }

        #endregion

    }
}