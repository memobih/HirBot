
using HirBot.Comman.Idenitity;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project.ResponseHandler.Models;
using skill.services.DataTransferObjects;
using skill.services.Interfaces;

namespace skill.api.Controllers
{


    [Route("api/[controller]")]
    [ApiController]

    public class SkillController : ApiControllerBase
    {
        private readonly ILogger<SkillController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISkillService _skillService;

        public SkillController(ILogger<SkillController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ISkillService skillService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _skillService = skillService;
        }
        #region by admin 

        [HttpPost("AddSkill")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> AddSkill(AddSkillDto skill)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
               .SelectMany(v => v.Errors)
               .Select(e => e.ErrorMessage)
               .ToList();
                return BadRequest(new APIOperationResponse<AddSkillDto>
                {
                    Message = "Invalid model state",
                    Succeeded = false,
                    Errors = errors,
                    Data = skill
                });
            }
            
            var result = await _skillService.AddSkill(skill);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("GetAllSkills")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSkills()
        {
            var result = await _skillService.GetAllSkills();
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        [HttpDelete("DeleteSkill/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteSkill(int id)
        {
            var result = await _skillService.DeleteSkill(id);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("GetSkill/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSkill(int id)
        {
            var result = await _skillService.GetSkill(id);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("UpdateSkill")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSkill(UpdateSkillDto skill)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
               .SelectMany(v => v.Errors)
               .Select(e => e.ErrorMessage)
               .ToList();
                return BadRequest(new APIOperationResponse<UpdateSkillDto>
                {
                    Message = "Invalid model state",
                    Succeeded = false,
                    Errors = errors,
                    Data = skill
                });
            }
            var result = await _skillService.UpdateSkill(skill);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region BY User 
        [HttpGet]
        public async Task<IActionResult> GetAllSkillsFORUser()
        { 
            var result= await _skillService.GetALLSkillForUsers();    
            if(result.StatusCode==200) 
                return Ok(new {status=true , result.Data , result.Message});
            return StatusCode(result.StatusCode , new { status = false, result.Message });
        }

        #endregion


    }
}