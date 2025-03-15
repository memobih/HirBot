using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
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
        public Task<APIOperationResponse<object>> GetALLAplications();
        public Task<APIOperationResponse<object>> GetALLAplications(int jobid);

    }
}
