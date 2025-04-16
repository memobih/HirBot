using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<APIOperationResponse<GetInterviewDto>> UpdateAsync(int id, InterviewDto dto);
        Task<APIOperationResponse<bool>> DeleteAsync(int id);
    }
}
