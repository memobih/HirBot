
using Project.Repository.Repository;
using User.Services.Interfaces;
using Project.Services.Interfaces;
using HirBot.ResponseHandler.Models;
using User.Services.DataTransferObjects.Profile;
using Microsoft.EntityFrameworkCore;
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
            if (user.Portfolio!=null)
            {
                profile.PortfolioUrl= user.Portfolio.PortfolioUrl;
                profile.GithubUrl= user.Portfolio.GithubUrl;
                profile.CVUrl=user.Portfolio.CVUrl;
                profile.Title = user.Portfolio.Title;
                profile.location = user.Portfolio.location;
            }
            var CurrentJop = _unitOfWork._context.Experiences.FirstOrDefault(e => e.ID ==user.CurentJopID);
            profile.Email = user.Email;
            profile.ContactNumber = user.PhoneNumber;
            profile.ProfileImagePath = user.ImagePath;
            profile.CoverImagePath = user.CoverPath;
            profile.FullName = user.FullName;
            profile.CurrentJop = CurrentJop;
            return APIOperationResponse<ProfileDto>.Success(profile);
        }
    }
}
