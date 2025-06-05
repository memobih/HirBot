using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;

namespace Jop.Services.Interfaces
{
    public interface IInterviewService
    {
        Task<APIOperationResponse<List<GetInterviewDto>>> GetAllAsync();
        Task<APIOperationResponse<GetInterviewDto>> GetByIdAsync(int id);
        Task<APIOperationResponse<GetInterviewDto>> CreateAsync(InterviewDto dto);
        Task<APIOperationResponse<GetInterviewDto>> UpdateAsync( InterviewDto dto);
        Task<APIOperationResponse<bool>> DeleteAsync(int id);
        Task<APIOperationResponse<InterviewCandidateinfoDto>> GetInterviewCandidateInfoAsync(string applicationId);
        Task<APIOperationResponse<object>>GetExamByInterviewIdAsync(string interviewId);
        
    }
}
