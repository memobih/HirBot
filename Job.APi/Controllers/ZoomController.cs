using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jop.Services.Implemntations;
using Microsoft.AspNetCore.Mvc;

namespace Job.APi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZoomController : ControllerBase
    {
        private readonly ZoomMeetingService _zoomService;

        public ZoomController(ZoomMeetingService zoomService)
        {
            _zoomService = zoomService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateZoomMeeting(DateTime time, string topic)
        {
            var link = await _zoomService.CreateMeetingAsync(time, topic);
            return Ok(new { link });
        }
    }
}