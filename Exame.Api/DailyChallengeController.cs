﻿using Exame.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyChallengeController : ApiControllerBase
    {
        private readonly IDailyChanalgeService _dailyChanalgeService;
        public DailyChallengeController(IDailyChanalgeService dailyChanalgeService)
        {
            _dailyChanalgeService = dailyChanalgeService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getALL()
        {
            var response = await _dailyChanalgeService.GetAll();
            if (response.StatusCode == 200)
                return Ok(new { status = true, data = response.Data });
            return StatusCode(response.StatusCode, new { status = false, Message = response.Message });

        }
        [Authorize]
        [HttpGet("{exameID}")]
        public async Task<IActionResult> GetExame(int exameID)
        {
            var response = await _dailyChanalgeService.GetDailyChanalge(exameID);
            if (response.StatusCode == 200)
                return Ok(new { status = true, data = response.Data });
            return StatusCode(response.StatusCode, new { status = false, Message = response.Message });

        }

    }
}