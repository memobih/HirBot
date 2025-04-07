using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;

namespace Jop.Services.Implemntations
{
    public class InterviewService : IInterviewService
    {
         private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly UnitOfWork unitOfWork;
        public InterviewService(Project.Services.Interfaces.IAuthenticationService authenticationService, UnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<APIOperationResponse<object>> SchuduleInitialInterview(InitialInterviewDto interviewDto)
        {
            var userc = _authenticationService.GetCurrentUserAsync().Result;
           var company=await unitOfWork.Companies.GetLastOrDefaultAsync(c=>c.UserID==userc.Id);
           if(company==null)
           {
               return new APIOperationResponse<object>()
               {
                   StatusCode = 400,
                   Message = "this user is not a company",
                   Data = null
               };
           }
           var appuser= await unitOfWork.Users.GetLastOrDefaultAsync(c=>c.Id==interviewDto.ApplicantId);
              if(appuser==null)
              {
                return  APIOperationResponse<object>.NotFound("this user is not a applicant");
              }
                var isusernotanapplicant=await unitOfWork._context.Applications.Include(c=>c.User).LastOrDefaultAsync(c=>c.ID==interviewDto.ApplicationId && c.UserID==interviewDto.ApplicantId);
                    if(isusernotanapplicant==null)
                    {
                        return  APIOperationResponse<object>.NotFound("this user is not a applicant for this job");
                    }
            var isuserhaveinterviewinthesametime=await unitOfWork._context.Interviews.Include(c=>c.Application).LastOrDefaultAsync(c=>c.Time==interviewDto.InterviewTime && c.Date==interviewDto.InterviewDate && c.Application.UserID==interviewDto.ApplicantId);
        
            if(isuserhaveinterviewinthesametime!=null)
            {
                return new APIOperationResponse<object>()
                {
                    StatusCode = 400,
                    Message = "this user have interview in this time",
                    Data = null
                };
            }
            try 
            {
                var interview = new Interview
                {
                    InterviewType = interviewDto.InterviewType,
                    InterviewerName = interviewDto.InterviewerName,
                    Date = interviewDto.InterviewDate,
                    Time = interviewDto.InterviewTime,
                    Location = interviewDto.InterviewLocation,
                    Notes = interviewDto.Notes,
                    ApplicationID = interviewDto.ApplicationId
               
                };
                await unitOfWork._context.Interviews.AddAsync(interview);
                await unitOfWork._context.SaveChangesAsync();
                return new APIOperationResponse<object>()
                {
                    StatusCode = 200,
                    Message = "Initial Interview scheduled successfully",
                    Data = interview
                };
            }
            catch (Exception ex)
            {
                return new APIOperationResponse<object>()
                {
                    StatusCode = 500,
                    Message = "An error occurred while scheduling the interview: " + ex.Message,
                    Data = null
                };
            }

        }


        public async Task<APIOperationResponse<object>> SchuduleTechnicalInterview(TechnicalInterviewDto interviewDto)
        {
           var userc = _authenticationService.GetCurrentUserAsync().Result;
           var company=await unitOfWork.Companies.GetLastOrDefaultAsync(c=>c.UserID==userc.Id);
           if(company==null)
           {
               return new APIOperationResponse<object>()
               {
                   StatusCode = 400,
                   Message = "this user is not a company",
                   Data = null
               };
           }
              var appuser= await unitOfWork.Users.GetLastOrDefaultAsync(c=>c.Id==interviewDto.ApplicantId);
              if(appuser==null)
              {
                return  APIOperationResponse<object>.NotFound("this user is not a applicant");
              }
              var isusernotapplicant=await unitOfWork._context.Applications.Include(c=>c.User).LastOrDefaultAsync(c=>c.ID==interviewDto.ApplicationId && c.UserID==interviewDto.ApplicantId);
                if(isusernotapplicant==null)
                {
                    return  APIOperationResponse<object>.NotFound("this user is not a applicant for this job");
                }
             
              var isuserhaveinterviewinthesametime=await unitOfWork._context.Interviews.Include(c=>c.Application).LastOrDefaultAsync(c=>c.Time==interviewDto.InterviewTime && c.Date==interviewDto.InterviewDate && c.Application.UserID==interviewDto.ApplicantId);
        
            if(isuserhaveinterviewinthesametime!=null)
            {
                return new APIOperationResponse<object>()
                {
                    StatusCode = 400,
                    Message = "this user have interview in this time",
                    Data = null
                };
            }
            try 
            {
                var interview = new Interview
                {
                    InterviewType = interviewDto.InterviewType,
                    InterviewerName = interviewDto.InterviewerName,
                    Date = interviewDto.InterviewDate,
                    Time = interviewDto.InterviewTime,
                    Location = interviewDto.InterviewLocation,
                    Notes = interviewDto.Notes,
                    ApplicationID = interviewDto.ApplicationId
               
                };
                await unitOfWork._context.Interviews.AddAsync(interview);
                await unitOfWork._context.SaveChangesAsync();
                return new APIOperationResponse<object>()
                {
                    StatusCode = 200,
                    Message = "Technical Interview scheduled successfully",
                    Data = interview
                };
            }
            catch (Exception ex)
            {
                return new APIOperationResponse<object>()
                {
                    StatusCode = 500,
                    Message = "An error occurred while scheduling the interview: " + ex.Message,
                    Data = null
                };
            }

        }
        public Task<bool> StartInterviewProcess(int jobId)
        {
            throw new NotImplementedException();
        }

    }
    
}