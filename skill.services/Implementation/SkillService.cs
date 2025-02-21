using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using HirBot.Redies;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Project.Repository.Repository;
using skill.services.DataTransferObjects;
using skill.services.Interfaces;
using User.Services.models;
namespace skill.services.Implementation
{
    public class SkillService : ISkillService
    {
          private readonly UserManager<ApplicationUser> _userManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SkillService(UserManager<ApplicationUser> userManager, UnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _signInManager = signInManager;
        }
        public Task<APIOperationResponse<AddSkillDto>> AddSkill(AddSkillDto skill)
        {
              var user=GetCurrentUserAsync().Result;
              if (user == null)
              {
                  return Task.FromResult(new APIOperationResponse<AddSkillDto>
                  {
                      Message = "User not found",
                      Succeeded = false
                  });
              }
                var skillModel = new Skill
                {
                    Name = skill.Name,
                    Status = skill.Status,
                    ImagePath = skill.ImagePath,
                    CreatedBy = user.FullName,
                    CreationDate = DateTime.Now,
                };
                _unitOfWork._context.Skills.Add(skillModel);
                try
                {
                    _unitOfWork._context.SaveChanges();
                    return Task.FromResult(new APIOperationResponse<AddSkillDto>
                    {
                        Message = "Skill added successfully",
                        Succeeded = true
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