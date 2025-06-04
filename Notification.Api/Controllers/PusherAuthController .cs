using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Comman.Idenitity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.ResponseHandler.Models;
using Project.Services.Interfaces;
using PusherServer;

namespace Notification.Api.Controllers
{
    [ApiController]
    [Route("api/broadcasting/auth")]
    [Authorize]
    public class PusherAuthController : ApiControllerBase
    {
         private readonly Pusher _pusher;
        private readonly IAuthenticationService _authService; 

        public PusherAuthController(Pusher pusher, IAuthenticationService authService)
        {
            _pusher = pusher;
            _authService = authService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Auth([FromForm] string channel_name, [FromForm] string socket_id)
        {
            var currentUser = await _authService.GetCurrentUserAsync();

            if (currentUser == null)
            {
                return Unauthorized("User not found");
            }

            // Now you have access to all user properties
            var userId = currentUser.Id; // or currentUser.UserName, currentUser.Email, etc.

            // Check if the user is allowed to subscribe to this channel
            if (!await IsUserAuthorizedForChannel(currentUser, channel_name))
            {
                return Forbid("Access denied to this channel");
            }

            // Generate auth signature
            var auth = _pusher.Authenticate(channel_name, socket_id);
            
            return Ok(auth);
        }

        private async Task<bool> IsUserAuthorizedForChannel(ApplicationUser user, string channelName)
        {
            // Example authorization logic using the full user object
            switch (channelName)
            {
                case var ch when ch.StartsWith("private-user-"):
                    // Extract user ID from channel name and compare
                    var channelUserId = ch.Replace("private-user-", "");
                    return channelUserId == user.Id;

                case var ch when ch.StartsWith("private-admin-"):
                    // Check if user has admin role
                    return await IsUserInRole(user, "Admin");

                case var ch when ch.StartsWith("private-company-"):
                    // Check if user belongs to the company
                    var companyId = ch.Replace("private-company-", "");
                    return user.Company.ID == companyId;

                default:
                    return false;
            }
        }

        private async Task<bool> IsUserInRole(ApplicationUser user, string roleName)
        {
        
            return user.role.ToString().Equals(roleName, StringComparison.OrdinalIgnoreCase);
        }
    }
    
}