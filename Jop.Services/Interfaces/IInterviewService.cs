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
        Task<APIOperationResponse<List<Interview>>> GetAllAsync();
        Task<APIOperationResponse<Interview>> GetByIdAsync(int id);
        Task<APIOperationResponse<Interview>> CreateAsync(InterviewDto dto);
        Task<APIOperationResponse<Interview>> UpdateAsync(int id, InterviewDto dto);
        Task<APIOperationResponse<bool>> DeleteAsync(int id);
    }
}
