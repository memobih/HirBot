using HirBot.ResponseHandler.Models;
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

    }
}
