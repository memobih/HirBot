using Exame.Services.DataTransferObjects;
using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Interfaces
{
    public interface IQuestionService
    { 
        public Task<APIOperationResponse<object>> GetQuestion(int questionId);
        public Task<APIOperationResponse<object>> Edit (int  questionId , QuestionDto dto );
        public Task<APIOperationResponse<object>> Create(int examID, QuestionDto dto);
        public Task<APIOperationResponse<object>> Delete(int questionId);


    }
}
