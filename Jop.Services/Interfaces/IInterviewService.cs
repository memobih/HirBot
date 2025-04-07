using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;

namespace Jop.Services.Interfaces
{
    public interface IInterviewService
    {
        public Task<bool>StartInterviewProcess(int jobId);
        public Task<APIOperationResponse<object>>SchuduleInitialInterview(InitialInterviewDto interviewDto);
        public Task<APIOperationResponse<object>>SchuduleTechnicalInterview(TechnicalInterviewDto interviewDto);
    }
}