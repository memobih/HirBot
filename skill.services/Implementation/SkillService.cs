
using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.Data.Interfaces;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Project.Repository.Repository;
using skill.services.DataTransferObjects;
using skill.services.Interfaces;

namespace skill.services.Implementation
{
    public class SkillService : ISkillService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IImageHandler _imageHandler;
        public SkillService(UserManager<ApplicationUser> userManager, UnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, SignInManager<ApplicationUser> signInManager, IImageHandler imageHandler)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _signInManager = signInManager;
            _imageHandler = imageHandler;
        }
        public Task<APIOperationResponse<AddSkillDto>> AddSkill(AddSkillDto skill)
        {
            var user = GetCurrentUserAsync().Result;
            if (user == null)
            {
                return Task.FromResult(new APIOperationResponse<AddSkillDto>
                {
                    Message = "User not found",
                    Succeeded = false
                });
            }
           var skillExist = _unitOfWork._context.Skills.FirstOrDefault(x => x.Name == skill.Name);
            if (skillExist != null)
            {
                return Task.FromResult(new APIOperationResponse<AddSkillDto>
                {
                    Message = "Skill already exist",
                    Succeeded = false
                });
            }
            if(skill.ImagePath == null)
            {
                return Task.FromResult(new APIOperationResponse<AddSkillDto>
                {
                    Message = "Image is required",
                    Succeeded = false
                });
            }
            var image = _imageHandler.UploadImageAsync(skill.ImagePath,"Skills").Result;
            if (!image.IsSuccess)
            {
                return Task.FromResult(new APIOperationResponse<AddSkillDto>
                {
                    Message = image.ErrorMessage,
                    Succeeded = false
                });
            }
            string imagePath = image.FilePath;
            var skillModel = new Skill
            {
                Name = skill.Name,
                Status = skill.Status,
                ImagePath = imagePath,
               // CreatedBy = user.FullName,
                CreationDate = DateTime.Now,
            };
            _unitOfWork._context.Skills.Add(skillModel);
            try
            {
                _unitOfWork._context.SaveChanges();
                return Task.FromResult(new APIOperationResponse<AddSkillDto>
                {
                    Message = "Skill added successfully",
                    Succeeded = true,
                    Data = skill
                });
            }
            catch (Exception e)
            {
                return Task.FromResult(new APIOperationResponse<AddSkillDto>
                {
                    Message = e.Message,
                    Succeeded = false
                });
            }

        }
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var currentUser = _contextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }
    }
}