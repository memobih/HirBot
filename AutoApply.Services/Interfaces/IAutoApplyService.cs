using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoApply.Services.Interfaces
{
    public  interface IAutoApplyService
    { 
        public Task<APIOperationResponse<object>> ChangeStatus(bool IsAuto);
        public Task<APIOperationResponse<object>> GetJobByAutoApply(string? search = null, string? experience = null, string? location = null, List<LocationType>? locationType = null, List<EmployeeType>? JobType = null, int page = 1, int perpage = 10, int? minSalary = null, int? maxSalary = null);
        public Task<APIOperationResponse<object>> GetStatus();

    }
}
