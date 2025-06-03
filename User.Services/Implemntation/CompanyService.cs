using HirBot.Comman.Enums;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Prepare;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Interfaces;
using User.Services.Response;

namespace User.Services.Implemntation
{
    public class CompanyService : ICompanyService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        public CompanyService(UnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }
        public async Task<APIOperationResponse<object>> ChangeStatus(CompanyStatus status, string id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized();
                var company =  _unitOfWork._context.Companies.Include(c=>c.account).Where(c => c.ID == id).FirstOrDefault();
                if (company != null)
                {

                    if (status == CompanyStatus.accepted) company.account.IsVerified = true;
                    company.status = status;
                    await _unitOfWork.Companies.UpdateAsync(company);
                    await _unitOfWork.Users.UpdateAsync(user);
                    await _unitOfWork.SaveAsync();
                }
                else return APIOperationResponse<object>.NotFound("the company is not found");

                return APIOperationResponse<object>.Success("the company is updated"  , "the company is updated");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }

        }

            public async Task<APIOperationResponse<object>> Delete(List<string> ids)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized();
                var companies =  _unitOfWork._context.Companies.Where(c=>ids.Contains(c.ID));
                    
       _unitOfWork._context.Companies.RemoveRange(companies);
                  await _unitOfWork.SaveAsync();
                
                return APIOperationResponse<object>.Success("the company is deleted", "the company is deleted");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> GetAllCompanies(string? search = null, CompanyStatus? status = null, int page = 1, int perpage = 10)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized();
                var companies = _unitOfWork._context.Companies.Include(C => C.account).Where(c =>c.ID!="0");
                var response = new List<CompanyList>();
                foreach (var company in companies)
                {
                    response.Add(new CompanyList
                    {
                        id = company.ID ,
                        name = company.account.FullName,

                        email = company.account.Email,

                        contact = company.account.PhoneNumber,

                        category = company.CompanyType,


                        country = company.country,

                        logo = company.account.ImagePath,

                        TaxNumber = company.TaxIndtefierNumber,

                        status = company.status
                    }

                       ); 
                }
                Filter(ref response, search , status);
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (response.Count() / perpage) + 1, pageSize = perpage, totalRecords = response.Count(), data = Paginate(response, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured"); 
            }
        }

        public async Task<APIOperationResponse<object>> GetByid(string id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Admin)
                    return APIOperationResponse<object>.UnOthrized();
                var company = await _unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.ID == id , c=>c.account);
                if (company != null)
                {
                    var response = new CompanyDetails(); 
                    response.name=company.account.FullName; 
                    response.phoneNumber = company.account.PhoneNumber;
                    response.country = company.country; 
                    response.Governate=company.Governate;
                    response.street = company.street;
                    response.email = company.account.Email; 
                    response.logo=company.account.ImagePath;
                    response.TaxNumber = company.TaxIndtefierNumber;
                    response.id = company.ID;
                    response.status = company.status;
                    response.legalInformation.BuisnessLicense=company.BusinessLicense;
                 //   response.legalInformation.SocialMediaLink = company.SocialMeediaLink; 
                    response.legalInformation.website=company.websiteUrl;
                    return APIOperationResponse<object>.Success(response);
                }
                else return APIOperationResponse<object>.NotFound("the company is not found");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        private void Filter(ref List<CompanyList> companies, string? search = null , CompanyStatus ?status=null)
        {
            if(search!=null)
            companies = companies.Where(c =>
            c.name.StartsWith(search) ||
            c.email.StartsWith(search)
            ||
            c.contact.StartsWith(search)
            ||
            c.category.StartsWith(search)
            ||
            c.country.StartsWith(search)
          
            ).ToList(); 
            if(status!=null)
                companies=companies.Where(c=>c.status==status).ToList();

        }

        public async Task<APIOperationResponse<object>> GetAcceptedCompanies(string  ? search= null ,int page = 1, int perpage = 10)
        {
            try
            {
                var companies = _unitOfWork._context.Companies.Include(C => C.account).Where(c => c.status == CompanyStatus.accepted);
                if (search != null)
                    companies = companies.Where(c => c.Name.StartsWith(search));
                var response = new List<User.Services.Response.Company>();
                foreach (var company in companies)
                {
                    response.Add(new User.Services.Response.Company
                    {
                        id = company.ID,
                        name = company.account.FullName,
                        logo = company.account.ImagePath,

                    }

                       );
                }
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (response.Count() / perpage) + 1, pageSize = perpage, totalRecords = response.Count(), data = Paginate(response, page, perpage) });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
    }
}
