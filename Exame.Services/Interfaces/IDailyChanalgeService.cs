using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Interfaces
{
    public  interface IDailyChanalgeService
    { 
        Task<APIOperationResponse<object>>GetDailyChanalge(int id);
        Task<APIOperationResponse<object>> GetAll(); 
    }
}
