using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IAuthenticationService _authenticationService;

        public PusherAuthController(IConfiguration config, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            var options = new PusherOptions
            {
                Cluster = config["Pusher:Cluster"],
                Encrypted = true
            };

            _pusher = new Pusher(
                config["Pusher:AppId"],
                config["Pusher:Key"],
                config["Pusher:Secret"],
                options
            );
        }
        [HttpPost]
        public IActionResult Authenticate([FromForm] string channel_name, [FromForm] string socket_id)
        {

            var userId = User.Identity?.Name;
            var expectedChannel = $"private-user.{userId}";

            if (channel_name == expectedChannel)
            {
                var auth = _pusher.Authenticate(socket_id, channel_name);
                return new JsonResult(auth);
            }

            return Unauthorized();
        }

    }
}