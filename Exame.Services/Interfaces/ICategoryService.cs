using Exame.Services.DataTransferObjects;
using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<APIOperationResponse<object>> create(CategoryDto dto); 
        Task<APIOperationResponse<object>> update(int id , CategoryDto dto);
        Task<APIOperationResponse<object>> GetALL(string ? search=null);
        Task<APIOperationResponse<object>> Delete(int id );

    }
}
