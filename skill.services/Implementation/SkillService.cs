
using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.Data.Interfaces;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Project.Repository.Repository;
using skill.services.DataTransferObjects;
using skill.services.Interfaces;
using Project.Services.Interfaces;
namespace skill.services.Implementation
{
    public class SkillService : ISkillService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IImageHandler _imageHandler; 
        private readonly IAuthenticationService _authenticationService;
        public SkillService(UserManager<ApplicationUser> userManager, UnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, SignInManager<ApplicationUser> signInManager, IImageHandler imageHandler , IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _signInManager = signInManager;
            _imageHandler = imageHandler;
            _authenticationService = authenticationService;
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
            var skillModel = new HirBot.Data.Entities.Skill
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

        public async Task<APIOperationResponse<object>> GetALLSkillForUsers()
        {
            try
            {
                var skills = await _unitOfWork.Skills.GetAllAsync();
                return APIOperationResponse<Object>.Success(skills , "ALL skills ");
            }
            catch (Exception e)
            {
                return APIOperationResponse<Object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> AddSkill(int id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var skill = _unitOfWork._context.Skills.Where(s=>s.ID==id).FirstOrDefault();
                if (skill == null)
                    return APIOperationResponse<Object>.NotFound("this skill is not found");
                var userKill =  _unitOfWork._context.UserSkills.Where(s => s.SkillID == id && user.Id == s.UserID).FirstOrDefault();
                if (userKill == null) {
                    userKill = new UserSkill { SkillID = skill.ID, UserID = user.Id, Rate = 0 };
                       _unitOfWork._context.UserSkills.Add(userKill);
                    await _unitOfWork.SaveAsync();
                    return APIOperationResponse<Object>.Success("skill added susseful ", "skill added susseful ");
                }
                return APIOperationResponse<Object>.Conflict("this skill is already added"); 
            }
            catch (Exception e)
            {
                return APIOperationResponse<Object>.ServerError("there are error accured");
            }
        }
    }
}