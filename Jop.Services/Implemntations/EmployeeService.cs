using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
//using Jop.Services.Helpers;
using Jop.Services.Interfaces;
using Jop.Services.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Prepare;
using Org.BouncyCastle.Crypto;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Implemntations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployeeService(UnitOfWork unitOfWork, IAuthenticationService authenticationService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _userManager = userManager;
        }
        public async Task<APIOperationResponse<object>> AddEmployee(AddEmployeeDto employee)
        {
            try
            {
                var CompanyUser = await _authenticationService.GetCurrentUserAsync();
                var company = await _unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == CompanyUser.Id);

                if (company == null) return APIOperationResponse<object>.UnOthrized("this user is not company");
                var user = await _userManager.FindByEmailAsync(employee.email);
                if (user == null)
                {
                    return APIOperationResponse<object>.BadRequest("this email is not found ", new { email = "this email is not found" });
                }
                var newExperience = new Experience();
                var added = _unitOfWork._context.Experiences.FirstOrDefault(e => e.UserID == user.Id && e.CompanyID == company.ID);
                if (added == null)
                {
                    if (user == null)
                    {
                        return APIOperationResponse<object>.BadRequest("this email is already used ", new { email = "this email is already used" });
                    }
                }
                newExperience.Start_Date = employee.startDate;
                newExperience.employeeType = employee.jobType;
                newExperience.End_Date = employee.endDate;
                newExperience.Title = employee.title;
                newExperience.location = employee.location;
                newExperience.workType = employee.workType;
                newExperience.CompanyID=company.ID;
                newExperience.UserID = user.Id;
                newExperience.rate=employee.rate;
                newExperience.IsStill=true;
                newExperience.companyName = company.Name;
                newExperience.privacy = PrivacyEnum.Public;
                
                await _unitOfWork.Experiences.AddAsync(newExperience);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("employee added Successful", "employee added Successful");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.BadRequest("there are error accured");
            }


        }

        public async Task<APIOperationResponse<object>> GetAllEmployee(string? search = null, EmployeeType? jobType = null, LocationType? workType = null, int? rate = null, int page = 1, int perpage = 10)
        {
            try
            {
                var user=await  _authenticationService.GetCurrentUserAsync();
                var company = await _unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this user is not a company or accepted");
                var experiences = _unitOfWork._context.Experiences.Include(e => e.User).Where(e => e.CompanyID == company.ID&& e.IsStill==true).ToList();
                List<EmployeeList> employees = new List<EmployeeList>();
                foreach (Experience experience in experiences)
                {
                    employees.Add(
                        new EmployeeList
                        {
                            name = experience.User.FullName,
                            jobType = experience.employeeType,
                            workType = experience.workType,
                            Rate = experience.rate,
                            Title = experience.Title,
                            start_date = experience.Start_Date ,
                            id=experience.ID
                        }
                        ); 
                }
                Filter(ref employees , search , jobType , workType , rate );
                return APIOperationResponse<object>.Success(new { currentPage = page, totalPages = (employees.Count() / perpage) + 1, pageSize = perpage, totalRecords = employees.Count(), data = Paginate(employees, page, perpage) });
            }
            catch (Exception e)
            {
                return APIOperationResponse<object>.ServerError("there are error accured"); 
            }
        }
        public async Task<APIOperationResponse<object>> EditEmployee(int id, AddEmployeeDto employee)
        {
            try
            {
                var CompanyUser = await _authenticationService.GetCurrentUserAsync();
                var company = await _unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == CompanyUser.Id);
                var editedExperience = await _unitOfWork.Experiences.GetLastOrDefaultAsync(e => e.ID == id);

                if (company == null || editedExperience.CompanyID!=company.ID) 
                    return APIOperationResponse<object>.UnOthrized("this user is not company");
                var user = await _userManager.FindByEmailAsync(employee.email);
                if (user == null)
                {
                    return APIOperationResponse<object>.BadRequest("this email is not found ", new { email = "this email is not found" });
                }
       
                editedExperience.Start_Date = employee.startDate;
                editedExperience.employeeType = employee.jobType;
                editedExperience.End_Date = employee.endDate;
                editedExperience.Title = employee.title;
                editedExperience.location = employee.location;
                editedExperience.workType = employee.workType;
                editedExperience.CompanyID = company.ID;
                editedExperience.UserID = user.Id;
                editedExperience.rate = employee.rate;
                await _unitOfWork.Experiences.UpdateAsync(editedExperience);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("employee updated Successful", "employee updated Successful");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.BadRequest("there are error accured");
            }
        }

        public async  Task<APIOperationResponse<object>> DeleteEmployees(List<int> ids)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await _unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.UserID == user.Id);
                if (company == null || company.status != CompanyStatus.accepted)
                    return APIOperationResponse<object>.UnOthrized("this user is not a company or accepted");
                List<Experience> Employess = _unitOfWork._context.Experiences.Where(e => e.CompanyID == company.ID
                &&
                ids.Contains(e.ID)

                ).ToList();
                foreach (var employee in Employess)
                {
                    employee.IsStill = false;
                }
                _unitOfWork._context.UpdateRange(Employess);
              await  _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("employees is deleted succesful");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured"); 
            } 

        }

        public async Task<APIOperationResponse<object>> showEmployee(int id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await _unitOfWork.Companies.GetLastOrDefaultAsync(c => c.UserID == user.Id);
                var experience =await  _unitOfWork.Experiences.GetLastOrDefaultAsync(e => e.ID == id); 
                if (company == null || company.status != CompanyStatus.accepted || experience.CompanyID!=company.ID)
                    return APIOperationResponse<object>.UnOthrized("this user is can not show this employee");

                var emp = new EmployeeList();
                emp.id = experience.ID;
             
                emp.name = (await _unitOfWork.Users.GetLastOrDefaultAsync(u => u.Id == experience.UserID)).FullName;
                emp.Title = experience.Title;
                emp.jobType = experience.employeeType;
                emp.workType = experience.workType;
                emp.Rate = experience.rate;
                emp.start_date = experience.Start_Date;
                return APIOperationResponse<object>.Success(emp);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        #region Helper  
        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        private void Filter(ref List<EmployeeList> employees, string? search = null, EmployeeType? jobType = null, LocationType? workType = null, int? rate = null)
        {
            if (search != null && !search.IsNullOrEmpty())
            {
                employees = employees.
                    Where(
                    e => e.Title.StartsWith(search)
                    ||
                    e.name.StartsWith(search)
                    ).ToList();
 
                  
            }
            if (jobType != null)
                employees = employees.Where(j => j.jobType == jobType).ToList();
            if (workType != null)
                employees = employees.Where(j => j.workType == workType).ToList();
          

        }

     
        #endregion
    }
}
