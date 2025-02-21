
using HirBot.Comman.Idenitity;
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
        private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly ISkillService _skillService;

        public SkillController(ILogger<SkillController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, Project.Services.Interfaces.IAuthenticationService authenticationService, ISkillService skillService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _authenticationService = authenticationService;
            _skillService = skillService;
        }


        [HttpPost("AddSkill")]
        public async Task<IActionResult> AddSkill(AddSkillDto skill)
        {
            var result = await _skillService.AddSkill(skill);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
    }
}