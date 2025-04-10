using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.Interfaces
{
    public interface ICompanyService
    { 
        public Task<APIOperationResponse<object >>GetAllCompanies (string ?search=null , int page = 1, int perpage = 10);
        public Task<APIOperationResponse<object>> ChangeStatus(CompanyStatus status, string id); 

        public Task<APIOperationResponse<object>> GetByid(string id);
        public Task<APIOperationResponse<object>> Delete(string id);  


    }
}
