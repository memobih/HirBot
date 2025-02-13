using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;

namespace User.Services.Interfaces
{
    public  interface IExperienceServices
    {
        public Task<APIOperationResponse<object>> AddExperienceAsync(ExperienceDto experience);
        public Task<APIOperationResponse<object>> GetExperienceAsync();

    }
}
