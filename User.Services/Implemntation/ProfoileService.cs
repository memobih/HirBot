
using Project.Repository.Repository;
using User.Services.Interfaces;
using Project.Services.Interfaces;
using HirBot.ResponseHandler.Models;
using User.Services.DataTransferObjects.Profile;
using Microsoft.EntityFrameworkCore;
using User.Services.DataTransferObjects;
using User.Services.Response;
using HirBot.Comman.Enums;
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

        public async Task<APIOperationResponse<ProfileDto>> GetPorofileAync()
        {
            var user = await authenticationService.GetCurrentUserAsync();
            if (user == null )
                return APIOperationResponse<ProfileDto>.NotFound("NotFound");
            user = await _unitOfWork._context.users.Include(U => U.Portfolio).FirstOrDefaultAsync(u => u.Id == user.Id);
            ProfileDto profile = new ProfileDto();
            ContactInfo contactInfo =new ContactInfo();
            if (user.Portfolio!=null)
            {
                contactInfo.PortfolioURL= user.Portfolio.PortfolioUrl;
               contactInfo.GithubURL= user.Portfolio.GithubUrl;
                profile.CVUrl=user.Portfolio.CVUrl;
                profile.title = user.Portfolio.Title;
                contactInfo.Location = user.Portfolio.location;
            }
            var experience = _unitOfWork._context.Experiences.Include(e=>e.Company).FirstOrDefault(e => e.ID ==user.CurentJopID);
             ExperienceResponse CurrentJop= new ExperienceResponse();
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
            profile.ContactInfo= contactInfo;
        
            return APIOperationResponse<ProfileDto>.Success(profile);
        }
    }
}
