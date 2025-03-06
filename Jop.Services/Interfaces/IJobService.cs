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
        public Task<APIOperationResponse<object>> edit(int  id , AddJobDto job);

    }
}
