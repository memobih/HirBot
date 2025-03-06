using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;

namespace User.Services.Interfaces
{
    public  interface IEducationService
    {
        public Task<APIOperationResponse<object>> GetAllEducationAsync();
        public Task<APIOperationResponse<object>> AddEducationAsync(EducationDto education ); 

        public Task <APIOperationResponse<object>> EditEducationAsync (int id , EducationDto education);

        public Task<APIOperationResponse<object>> GetEducationByIdAsync (int id );
        public Task<APIOperationResponse<object>> DeleteEducationByIdAsync(int id);

    }
}
