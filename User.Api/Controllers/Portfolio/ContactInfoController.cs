using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Comman.Idenitity;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project.ResponseHandler.Models;
using User.Services.DataTransferObjects;
using User.Services.Interfaces;

namespace User.Api.Controllers.Portfolio
{
    [Route("[controller]")]
    public class ContactInfoController : ApiControllerBase
    {
        private readonly ILogger<ContactInfoController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Project.Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly IContactInfoService _contactInfoService;

        public ContactInfoController(ILogger<ContactInfoController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, Project.Services.Interfaces.IAuthenticationService authenticationService, IContactInfoService contactInfoService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _authenticationService = authenticationService;
            _contactInfoService = contactInfoService;
        }
        [HttpGet("contact-info")]
        [Authorize]
        public async Task<APIOperationResponse<ContactInfoDto>> GetContactInfoAsync()
        {
            return await _contactInfoService.GetContactInfoAsync();
        }
        [HttpPut("contact-info-update")]
        [Authorize]
        public async Task<APIOperationResponse<ContactInfoDto>> UpdateContactInfoAsync(ContactInfoDto contactInfoDto)
        {
            return await _contactInfoService.UpdateContactInfoAsync(contactInfoDto);
        }

    }
}