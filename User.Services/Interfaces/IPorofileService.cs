using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;

namespace User.Services.Interfaces
{
    public  interface IPorofileService
    {
        public Task<APIOperationResponse<ProfileDto>> GetPorofileAync();
        public Task<APIOperationResponse<object>> GetCompanyProfileAsync();  
        public Task<APIOperationResponse<object>> GetProfileWithUserName(string userName);
        public Task<APIOperationResponse<object>> UpdateCurrentJob(int id);
        public Task<APIOperationResponse<object>> DeleteCurrentJob();
        public  Task<APIOperationResponse<object>> GetCvAync();

    }
}
