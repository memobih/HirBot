using HirBot.ResponseHandler.Models;
using Skill.services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill.services.Interfaces
{
    public  interface ILevelService
    {
        public Task<APIOperationResponse<object>> GetALLLevel();
        public Task<APIOperationResponse<object>> GetALLLevelByAdmin(int page=1 , int perpage=10);
        public Task<APIOperationResponse<object>> AddLevel(LevelDto level);

        public Task<APIOperationResponse<object>> Delete(List<int> ids);
        public Task<APIOperationResponse<object>> UpdateLevel (int id , LevelDto level);


    }
}
