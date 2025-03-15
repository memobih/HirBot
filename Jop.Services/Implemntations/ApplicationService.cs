using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.Interfaces;
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
              return  APIOperationResponse<object>.Success("You are Applied Succefuly");

            }
            catch(Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there  are error accured ");
            }
        }
        public async Task<APIOperationResponse<object>> GetALLAplications()
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there  are error accured ");
            }
        }

        public async  Task<APIOperationResponse<object>> GetALLAplications(int jobid)
        {
            try
            {


            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there  are error accured ");
            }
        }
    }
}
