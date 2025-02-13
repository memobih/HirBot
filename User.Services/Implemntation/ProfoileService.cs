
using Project.Repository.Repository;
using User.Services.Interfaces;
using Project.Services.Interfaces;
using HirBot.ResponseHandler.Models;
using User.Services.DataTransferObjects.Profile;
using Microsoft.EntityFrameworkCore;
using User.Services.DataTransferObjects;
using User.Services.Response;
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
            if (user == null)
                return APIOperationResponse<ProfileDto>.NotFound("NotFound");
            user = await _unitOfWork._context.users.Include(U => U.Portfolio).FirstOrDefaultAsync(u => u.Id == user.Id);
            ProfileDto profile = new ProfileDto(); 
            ContactInfoDto contactInfo=new ContactInfoDto();
            if (user.Portfolio!=null)
            {
                contactInfo.PortfolioURL= user.Portfolio.PortfolioUrl;
               contactInfo.GithubUrl= user.Portfolio.GithubUrl;
                profile.CVUrl=user.Portfolio.CVUrl;
                profile.Title = user.Portfolio.Title;
                contactInfo.Location = user.Portfolio.location;
            }
            var experience = _unitOfWork._context.Experiences.Include(e=>e.Company).FirstOrDefault(e => e.ID ==user.CurentJopID);
             ExperienceResponse CurrentJop= new ExperienceResponse();
            if (experience != null)
            {

                CurrentJop.privacy = experience.privacy;
                CurrentJop.Start_Date = experience.Start_Date;
                CurrentJop.employeeType = experience.employeeType;
                CurrentJop.End_Date = experience.End_Date;
                CurrentJop.Title = experience.Title;
                CurrentJop.Location = experience.Location;
                CurrentJop.id = experience.ID;
                if (experience.Company != null)
                {
                    var name = _unitOfWork._context.users.First(u => u.CompanyID == experience.CompanyID).FullName;
                    CurrentJop.CompanyName = name;
                    CurrentJop.Logo = experience.Company.Logo;
                }
            }
            contactInfo.ContactNumber = user.PhoneNumber;
            profile.ProfileImagePath = user.ImagePath;
            profile.CoverImagePath = user.CoverPath;
            profile.FullName = user.FullName;
            profile.CurrentJop = CurrentJop;
            profile.ContactInfo= contactInfo;
        
            return APIOperationResponse<ProfileDto>.Success(profile);
        }
    }
}
