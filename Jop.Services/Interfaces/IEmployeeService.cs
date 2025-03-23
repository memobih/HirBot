using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Interfaces
{
     public  interface IEmployeeService
    {
        public Task<APIOperationResponse<object>> AddEmployee(AddEmployeeDto employee);
        public Task<APIOperationResponse<object>> GetAllEmployee(string ?search=null , EmployeeType  ? jobType=null ,LocationType ?workType  =null , int  ?rate=null , int page=1 , int perPage=10   );
    }
}
