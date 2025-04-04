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
        public Task<APIOperationResponse<Object>> DoExame(int skillId);
        public Task<APIOperationResponse<object>> FinishExame(int id, List<AnswerDto> answers);
    }
}
