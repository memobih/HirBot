using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Interfaces
{
    public  interface IJobService
    {
        public Task<APIOperationResponse<object>> AddJop(AddJobDto job);
        public Task<APIOperationResponse<object>> JobDetails(int id);
        public Task<APIOperationResponse<object>> JobDetailsCompany(int id);

        public Task<APIOperationResponse<Object>> DeleteJobs(List<int> ids);
        public Task<APIOperationResponse<object>> edit(int  id , AddJobDto job);
        public Task<APIOperationResponse<object>> getAllJobs(string? search = null, JobStatus? status = null, LocationType? locationType = null, EmployeeType? JobType = null, int page = 1, int perpage = 10, string? sort = "salary", string sortDirection = null); 
        public Task<APIOperationResponse<object>> getAllDraftedJobs(string? search = null, JobStatus? status = null, LocationType? locationType = null, EmployeeType? JobType = null, int page = 1, int perpage = 10, string? sort = "salary", string sortDirection = null);
        public Task<APIOperationResponse<object>> GetJosRecomendations(string? search = null,string ? experience =null , string ?location =null ,  List<LocationType>? locationType = null, List<EmployeeType>? JobType = null, int page = 1, int perpage = 10 , int ?minSalary  =null , int ?maxSalary=null );
        public Task<APIOperationResponse<object>> EditJobStatus(int id, EditStatusDto status);


    }
}
