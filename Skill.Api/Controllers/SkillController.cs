
using HirBot.Comman.Idenitity;
using HirBot.ResponseHandler.Models;
using Mailing;
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
            return BadRequest( new { status = false, Massage = result.Message });

            }
           return Ok( new { status = true, Massage = result.Message });

        }
        [HttpGet("GetAllSkills")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSkills(string? searh = null)
        {
            var result = await _skillService.GetAllSkills();
            if (!result.Succeeded)
            {
                return BadRequest(new { status = false, Massage = result.Message });

            }
            return Ok(new { status = true, data = result.Data });
        }
        [HttpDelete("DeleteSkill")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteSkill(List<int> ids)
        {
            var result = await _skillService.DeleteSkill(ids);
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
        [HttpPut("UpdateSkill/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSkill(int id  ,  UpdateSkillDto skill)
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
            var result = await _skillService.UpdateSkill(id  ,skill);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region BY User 
        [HttpGet("GetDeletedUserSkills")]
        [Authorize]
        public async Task<IActionResult> GetDeletedUserSkills()
        {
            var result = await _skillService.GetDeletedUserSkill();
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Data, result.Message });

            return StatusCode(result.StatusCode, new { status = false, result.Message });
        }
        [HttpPut("RestoreUserSkill/{id}")]
        [Authorize]
        public async Task<IActionResult> RestoreUserSkill(int id)
        {
            var result = await _skillService.RestoreUserSkill(id);
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Message });

            return StatusCode(result.StatusCode, new { status = false, result.Message });
        }
        [HttpDelete("DeleteUserSkill/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserSkill(int id)
        {
            var result = await _skillService.DeleteUserSkill(id);
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Message });

            return StatusCode(result.StatusCode, new { status = false, result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkillsFORUser()
        { 
            var result= await _skillService.GetALLSkillForUsers();    
            if(result.StatusCode==200) 
                return Ok(new {status=true , result.Data , result.Message});
            return StatusCode(result.StatusCode , new { status = false, result.Message });
        }
        [Authorize]
        [HttpPost("{id}/addSkill")]
        public async Task<IActionResult> AddSkill(int id )
        {
            var result = await _skillService.AddSkill(id);
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Message });
            return StatusCode(result.StatusCode, new { status = false, result.Message });
        }
        [Authorize]
        [HttpGet("userskill")]
        public async Task<IActionResult> GetUserSkill()
        {
            var result = await _skillService.GetUserSkill();
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Data, result.Message });
            return StatusCode(result.StatusCode, new { status = false, result.Message });
        }
        #endregion


    }
}