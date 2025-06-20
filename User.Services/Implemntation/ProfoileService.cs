﻿
using Project.Repository.Repository;
using User.Services.Interfaces;
using Project.Services.Interfaces;
using HirBot.ResponseHandler.Models;
using User.Services.DataTransferObjects.Profile;
using Microsoft.EntityFrameworkCore;
using User.Services.DataTransferObjects;
using User.Services.Response;
using HirBot.Comman.Enums;
using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using Org.BouncyCastle.Crypto.Prng;
using HirBot.Data.Enums;
using Microsoft.Extensions.Azure;
namespace User.Services.Implemntation
{
    public class ProfoileService : IPorofileService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IAuthenticationService authenticationService;
        public ProfoileService(UnitOfWork unitOfWork , IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            this.authenticationService = authenticationService;
        }

        public async Task<APIOperationResponse<object>> GetCompanyProfileAsync()
        {
            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company)
                    APIOperationResponse<object>.UnOthrized("This email is not A company");
                var entity = await _unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.UserID == user.Id , c=>c.account);
                var company =GetCompanyProfile(entity); 
                
                if(company == null) return APIOperationResponse<object>.ServerError("ther are error accured");
                company.ContactInfo.email = user.Email;
                var employes = _unitOfWork._context.Experiences.Include(e=>e.User).Where(e => e.CompanyID == entity.ID && e.IsStill == true).ToList().TakeLast(4);
                foreach (var employ in employes)
                {
                    company.employes.Add(new EmployeeList
                    {
                        email=employ.User.Email , 
                        id=employ.ID ,
                        image=employ.User.ImagePath , 
                        end_date=employ.End_Date , 
                        start_date=employ.Start_Date ,
                        jobType=employ.employeeType , 
                        location=employ.location , 
                        name=employ.User.FullName , 
                        workType=employ.workType , 
                        Rate=employ.rate ,
                        Title=employ.Title ,

                    });


                }
                return APIOperationResponse<object>.Success(company);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured"); 
            }
        }

        public async Task<APIOperationResponse<ProfileDto>> GetPorofileAync()
        {
            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                if (user == null)
                    return APIOperationResponse<ProfileDto>.NotFound("NotFound");
                user = await _unitOfWork._context.users.Include(U => U.Portfolio).FirstOrDefaultAsync(u => u.Id == user.Id);
                var profile = GetUserProfile(user);
              if (profile == null) return APIOperationResponse<ProfileDto>.ServerError("ther are error accured ");
                return APIOperationResponse<ProfileDto>.Success(profile);

            }
            catch (Exception ex)
            {
                return APIOperationResponse<ProfileDto>.ServerError("ther are error accured ");
            }
        }
        public async Task<APIOperationResponse<object>> GetCvAync()
        {
            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                user = await _unitOfWork._context.users.Include(U => U.Portfolio).FirstOrDefaultAsync(u => u.Id == user.Id);
                if(user.Portfolio==null) user.Portfolio=new Portfolio();
                return APIOperationResponse<object>.Success(new { Cv=user.Portfolio.CVUrl });
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("ther are error accured ");
            }
        }

        public async Task<APIOperationResponse<object>> GetProfileWithUserName(string userName)
        {
            try
            {
                var user = await _unitOfWork.Users.GetEntityByPropertyWithIncludeAsync(u => u.UserName == userName , u=>u.Portfolio);
                if (user == null)
                    return APIOperationResponse<object>.NotFound("this user is not found");
                if(user.role==UserType.User)
                {
                    var profile=GetUserProfile(user);
                    var education = _unitOfWork._context.Educations.Where(e => e.UserID == user.Id && e.Privacy==PrivacyEnum.Public).ToList();
                    var experinces = _unitOfWork._context.Experiences.Where(e => e.UserID == user.Id && e.privacy== PrivacyEnum.Public).ToList();

                    if (profile == null) 
                        return APIOperationResponse<object>.ServerError("ther are error accured ");
                    return APIOperationResponse<object>.Success(new { profile, educations=education, experinces= experinces, role = "user" } );
                }

               if (user.role == UserType.Company)
               {
                    var entity = await _unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.UserID == user.Id, c => c.account);
                    var company = GetCompanyProfile(entity);
                    company.Documents = null;
                    if (company == null) return APIOperationResponse<object>.ServerError("ther are error accured");
                    return APIOperationResponse<object>.Success(new { company, role = "company" } );
                }
                return APIOperationResponse<object>.NotFound("this user is not found");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured "); 
            }
        }

        #region helper 
        private ProfileDto GetUserProfile(ApplicationUser user)
        {
            try
            {
                ProfileDto profile = new ProfileDto();
                ContactInfo contactInfo = new ContactInfo();
                if (user.Portfolio != null)
                {
                    contactInfo.PortfolioURL = user.Portfolio.PortfolioUrl;
                    contactInfo.GithubURL = user.Portfolio.GithubUrl;
                    profile.CVUrl = user.Portfolio.CVUrl;
                    profile.title = user.Portfolio.Title;
                    contactInfo.Location = user.Portfolio.location;
                }
             
              var experience = _unitOfWork._context.Experiences.Include(e => e.Company).FirstOrDefault(e => e.ID == user.CurentJopID);
                
               
                ExperienceResponse CurrentJop = new ExperienceResponse();
                if (experience != null)
                {

                    CurrentJop.privacy = experience.privacy;
                    CurrentJop.startDate = experience.Start_Date;
                    CurrentJop.jobType = experience.employeeType;
                    CurrentJop.endDate = experience.End_Date;
                    CurrentJop.title = experience.Title;
                    CurrentJop.workType = experience.workType;
                    CurrentJop.location = experience.location;
                    CurrentJop.id = experience.ID;
                    if (experience.Company != null)
                    {
                        CurrentJop.company = new ExpreienceCompany();

                        var name = _unitOfWork._context.Companies.Include(C => C.account).First(c => c.ID == experience.CompanyID).account.FullName;
                        CurrentJop.company.name = name;
                        CurrentJop.company.id = experience.CompanyID;
                        CurrentJop.company.logo = experience.Company.Logo;
                    }
                }
                else CurrentJop = null;
                contactInfo.ContactNumber = user.PhoneNumber;
                profile.profileImageSrc = user.ImagePath;
                profile.coverImageSrc = user.CoverPath;
                profile.FullName = user.FullName;
                profile.CurrentJop = CurrentJop;
                profile.ContactInfo = contactInfo;
                return profile;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private CompanyProfileDto GetCompanyProfile(HirBot.Data.Entities.Company entity)
        {
            try
            {
                var company = new CompanyProfileDto();
                company.CoverPath = entity.account.CoverPath;
                company.ImagePath = entity.account.ImagePath;
                company.status = entity.status;
                company.CompanyType = entity.CompanyType;
                company.SocialMedia=new SocialMedia {FacebookLink=entity.FacebookLink , TwitterLink=entity.TwitterLink , InstgrameLink=entity.InstgrameLink  , TikTokLink=entity.TikTokLink };

                company.ContactInfo = new Contact { country = entity.country, street = entity.street, Governate = entity.Governate, websiteUrl = entity.websiteUrl };
                company.Documents = new Documents { BusinessLicense = entity.BusinessLicense, TaxIndtefierNumber = entity.TaxIndtefierNumber };
          
               
                company.name = entity.account.FullName; 
                return company;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<APIOperationResponse<object>> UpdateCurrentJob(int id)
        {
            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                var experience=await _unitOfWork.Experiences.GetByIdAsync(id);
                if(experience !=null && experience.UserID==user.Id  )
                {
                    user.CurentJop = experience;
                    user.CurentJopID = experience.ID;
                    await _unitOfWork.Users.UpdateAsync(user);
                    await _unitOfWork.SaveAsync();
                    return APIOperationResponse<object>.Success("the current job updated succesful" , "the current job updated succesful");

                }
                return APIOperationResponse<object>.BadRequest("try again !");


            }
            catch
            {
                return APIOperationResponse<object>.ServerError("there are error accured "); 
            }
        }

        public async Task<APIOperationResponse<object>> DeleteCurrentJob()
        {
            try
            {
                var user = await authenticationService.GetCurrentUserAsync();
                    user.CurentJop = null;
                user.CurentJopID = null;
                    await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveAsync();

                return APIOperationResponse<object>.Success("the current job deleted succesful", "the current job deleted succesful");
            }
            catch
            {
                return APIOperationResponse<object>.ServerError("there are error accured ");
            }
        }

        #endregion
    }
}
