using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using Skill.services.DataTransferObjects;
using Skill.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Skill.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ApiControllerBase
    {
        private readonly ILevelService _levelService;
        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }
        [HttpGet]
        public async Task<IActionResult> GetALLLevel()
        {
            var result = await _levelService.GetALLLevel();
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Data, result.Message });
            return StatusCode(result.StatusCode, new { status = false, result.Message });

        }
        [HttpGet("levels")]
        [Authorize]
        public async Task<IActionResult> GetALLLevel(int page = 1, int perpage = 10)
        {
            var result = await _levelService.GetALLLevelByAdmin(page, perpage);
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Data, result.Message });
            return StatusCode(result.StatusCode, new { status = false, result.Message });

        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> update(int id  , LevelDto level)
        {
            var result = await _levelService.UpdateLevel(id , level);
            if (result.StatusCode == 200)
                return Ok(new { status = true,  result.Message });
            return StatusCode(result.StatusCode, new { status = false, result.Message });

        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> delete(List<int> ids)
        {
            var result = await _levelService.Delete(ids);
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Message });
            return StatusCode(result.StatusCode, new { status = false, result.Message });

        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> add(LevelDto level)
        {
            var result = await _levelService.AddLevel( level);
            if (result.StatusCode == 200)
                return Ok(new { status = true, result.Message });
            return StatusCode(result.StatusCode, new { status = false, result.Message });

        }
    }
}
