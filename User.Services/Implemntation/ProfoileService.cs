using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Profile;
using User.Services.Interfaces;

namespace User.Services.Implemntation
{
    public class ProfoileService : IPorofileService
    {
        private readonly UnitOfWork _unitOfWork;
        public ProfoileService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIOperationResponse<ProfileDto>> GetPorofileAync(string userId)
        {
          var user =await _unitOfWork._context.users.Include(U=>U.Portfolio).FirstOrDefaultAsync(u=>u.Id==userId);
            if (user == null)
                return APIOperationResponse<ProfileDto>.NotFound("NotFound");
            ProfileDto profile = new ProfileDto(); 
            if (user.Portfolio!=null)
            {
                profile.PortfolioUrl= user.Portfolio.PortfolioUrl;
                profile.GithubUrl= user.Portfolio.GithubUrl;
                profile.CVUrl=user.Portfolio.CVUrl;
                profile.Title = user.Portfolio.Title;
                profile.location = user.Portfolio.location;
            }
            profile.Email = user.Email;
            profile.ContactNumber = user.PhoneNumber;
            profile.ProfileImagePath = user.ImagePath;
            profile.CoverImagePath = user.CoverPath;
            profile.FullName = user.FullName;
            return APIOperationResponse<ProfileDto>.Success(profile);
        }
    }
}
