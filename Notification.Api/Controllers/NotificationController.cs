using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Services.DataTransferObjects;
using Notification.Services.Interfaces;
using Project.ResponseHandler.Models;
using StackExchange.Redis;

namespace Notification.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ApiControllerBase
    {
         private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserNotifications()
        {
            var result = await _notificationService.GetAllForUserAsync();
            if(result.StatusCode==200)
              return Ok(new {status=true , data = result.Data});
            else return StatusCode(result.StatusCode  , new {status=false , Message = result.Message});
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<APIOperationResponse<int>>> GetUnreadCount()
        {
            var result = await _notificationService.CountUnreadNotificationsAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("mark-as-read")]
        [Authorize]
        public async Task<ActionResult<APIOperationResponse<bool>>> MarkAsRead(List<int> ids)
        {
            var result = await _notificationService.MarkAsReadAsync(ids);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{notificationId}")]
        public async Task<ActionResult<APIOperationResponse<bool>>> DeleteNotification(int notificationId)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            return StatusCode(result.StatusCode, result);
        }

     
    }
}
