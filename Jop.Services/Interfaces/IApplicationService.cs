using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Interfaces
{
    public  interface IApplicationService
    {
        public Task<APIOperationResponse<object>> ApplicateOnJob(int jobId);
        public Task<APIOperationResponse<object>> GetALLAplications(int jobid, string? search = null, ApplicationStatus? status = null, string columnsort = "score" ,string? sort = null, int page = 1, int perpage = 10);

        public Task<APIOperationResponse<object>> ApproveApplocations(List<int> ids);
        public Task<APIOperationResponse<object>> RejectApplication(List<int> ids);
        public Task<APIOperationResponse<object>> DeleteApplications(List<int> ids);
        public Task<APIOperationResponse<object>> GetAllApprovedApplications(int jobId,string? search = null, string columnsort = "score", string? sort = null, int page = 1, int perpage = 10);
        public Task<APIOperationResponse<AppuserDto>>GetApplicantDetails(int ApplicationId);
        public Task<APIOperationResponse<object>> AcceptTheApplication(int ApplicationId);
        public Task<APIOperationResponse<object>> RejectTheApplication(int ApplicationId);
        public Task<APIOperationResponse<object>> TrackApprovedApplications();
        public Task<APIOperationResponse<object>> TrackRejectedApplications();
        public Task<APIOperationResponse<object>> TrackInprogressApplications();
        public Task<APIOperationResponse<object>> StartProcess(int ApplicationId);
        public Task<APIOperationResponse<object>>TrackAllApplications();
    }
}
