using Exame.Services.DataTransferObjects;
using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Interfaces
{
    public interface IExameService
    {
        public Task<APIOperationResponse<Object>> DoExame(UserSkillDto dto);
        public Task<APIOperationResponse<object>> GetALLExams(int categoryID);

        public Task<APIOperationResponse<object>> CreateExame(ExameDto exame);
        public Task<APIOperationResponse<object>> GetExame(int exameID);
        public Task<APIOperationResponse<object>> GetQuestions(int exameID);

        public Task<APIOperationResponse<object>> DeleteExame(int exameID);
        public Task<APIOperationResponse<object>> EditExame(int exameID, ExameDto exame);
        public Task<APIOperationResponse<object>> FinishExame(int id, List<AnswerDto> answers);

        public Task<APIOperationResponse<object>> FinishInterviewExame(int id, List<AnswerDto> answers);
        public Task<APIOperationResponse<object>> FinishSkillExame(int id, List<AnswerDto> answers);
        public Task<object> GetExamForinterview(int examid);

    }

}
