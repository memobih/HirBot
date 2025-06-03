using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Response;

namespace User.Services.Interfaces
{
    public interface ICompanyService
    { 
        public Task<APIOperationResponse<object >>GetAllCompanies (string ?search=null , CompanyStatus? status = null ,int page = 1, int perpage = 10);
        public Task<APIOperationResponse<object>> GetAcceptedCompanies(string ?  search=null , int page=1 , int perpage=10);

        public Task<APIOperationResponse<object>> ChangeStatus(CompanyStatus status, string id); 

        public Task<APIOperationResponse<object>> GetByid(string id);
        public Task<APIOperationResponse<object>> Delete(List<string> ids);
        public Task<APIOperationResponse<object>> EditBasicInformation(CompanyBasicInformationDto dto);
        public Task<APIOperationResponse<object>> EditSocialMedia(SocialMedia dto);
        public Task<APIOperationResponse<object>> EditContactInfo(Contact dto);
        public Task<APIOperationResponse<Object>> EditTaxID(Documents dto);
        public Task<APIOperationResponse<object>> GetBasicInformation( );
        public Task<APIOperationResponse<object>> GetSocialMedia( );
        public Task<APIOperationResponse<object>> GetContactInfo( );




    }
}
