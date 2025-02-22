
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
   
        public Task<APIOperationResponse<List<GettingAllSkillsDto>>> GetAllSkills()
        {
            var skills = _unitOfWork._context.Skills.ToList();
            if (skills.Count == 0)
            {
                return Task.FromResult(new APIOperationResponse<List<GettingAllSkillsDto>>
                {
                    Message = "No skills found",
                    Succeeded = false
                });
            }
            var skillsDto = new List<GettingAllSkillsDto>();
            foreach (var skill in skills)
            {
                var totalUsers = _unitOfWork._context.UserSkills.Count(x => x.SkillID == skill.ID);
                skillsDto.Add(new GettingAllSkillsDto
                {
                    ID = skill.ID,
                    Name = skill.Name,
                    ImagePath = skill.ImagePath,
                    TotalUsers = totalUsers,
                    CreatedAt = skill.CreationDate
                });
            }
            return Task.FromResult(new APIOperationResponse<List<GettingAllSkillsDto>>
            {
                Data = skillsDto,
                Succeeded = true
            });
        }
        public Task<APIOperationResponse<bool>> DeleteSkill(int id)
        {
            var skill = _unitOfWork._context.Skills.FirstOrDefault(x => x.ID == id);
            if (skill == null)
            {
                return Task.FromResult(new APIOperationResponse<bool>
                {
                    Message = "Skill not found",
                    Succeeded = false
                });
            }
            _unitOfWork._context.Skills.Remove(skill);
            try
            {
                _unitOfWork._context.SaveChanges();
                return Task.FromResult(new APIOperationResponse<bool>
                {
                    Message = "Skill deleted successfully",
                    Succeeded = true,
                    Data = true
                });
            }
            catch (Exception e)
            {
                return Task.FromResult(new APIOperationResponse<bool>
                {
                    Message = e.Message,
                    Succeeded = false
                });
            }
        }
        public Task<APIOperationResponse<GettingAllSkillsDto>> GetSkill(int id)
        {
            var skill = _unitOfWork._context.Skills.FirstOrDefault(x => x.ID == id);
            if (skill == null)
            {
                return Task.FromResult(new APIOperationResponse<GettingAllSkillsDto>
                {
                    Message = "Skill not found",
                    Succeeded = false
                });
            }
            var totalUsers = _unitOfWork._context.UserSkills.Count(x => x.SkillID == skill.ID);
            var skillDto = new GettingAllSkillsDto
            {
                ID = skill.ID,
                Name = skill.Name,
                ImagePath = skill.ImagePath,
                TotalUsers = totalUsers,
                CreatedAt = skill.CreationDate
            };
            return Task.FromResult(new APIOperationResponse<GettingAllSkillsDto>
            {
                Data = skillDto,
                Succeeded = true
            });
        }
        public Task<APIOperationResponse<UpdateSkillDto>> UpdateSkill(UpdateSkillDto skill)
        {
            var skillModel = _unitOfWork._context.Skills.FirstOrDefault(x => x.ID == skill.ID);
            if (skillModel == null)
            {
                return Task.FromResult(new APIOperationResponse<UpdateSkillDto>
                {
                    Message = "Skill not found",
                    Succeeded = false
                });
            }
            if (skill.ImagePath != null)
            {
                var result = _imageHandler.DeleteImage(skillModel.ImagePath).Result;
                if (!result.Item1)
                {
                    return Task.FromResult(new APIOperationResponse<UpdateSkillDto>
                    {
                        Message = result.Item2,
                        Succeeded = false
                    });
                }
                var image = _imageHandler.UploadImageAsync(skill.ImagePath, "Skills").Result;
                if (!image.IsSuccess)
                {
                    return Task.FromResult(new APIOperationResponse<UpdateSkillDto>
                    {
                        Message = image.ErrorMessage,
                        Succeeded = false
                    });
                }
                string imagePath = image.FilePath;
                skillModel.ImagePath = imagePath;
            }
            skillModel.Name = skill.Name;
            skillModel.Status = skill.Status;
            try
            {
                _unitOfWork._context.SaveChanges();
                return Task.FromResult(new APIOperationResponse<UpdateSkillDto>
                {
                    Message = "Skill updated successfully",
                    Succeeded = true,
                    Data = skill
                });
            }
            catch (Exception e)
            {
                return Task.FromResult(new APIOperationResponse<UpdateSkillDto>
                {
                    Message = e.Message,
                    Succeeded = false
                });
            }
        }
    }
}