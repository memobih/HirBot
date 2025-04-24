using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Mvc;
using Notification.Services.DataTransferObjects;
using Notification.Services.Interfaces;
using Project.ResponseHandler.Models;

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

        [HttpGet("{userId}")]
        public async Task<ActionResult<APIOperationResponse<List<NotificationDto>>>> GetUserNotifications(string userId)
        {
            var result = await _notificationService.GetAllForUserAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("unread-count/{userId}")]
        public async Task<ActionResult<APIOperationResponse<int>>> GetUnreadCount(string userId)
        {
            var result = await _notificationService.CountUnreadNotificationsAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("mark-as-read/{notificationId}/{userId}")]
        public async Task<ActionResult<APIOperationResponse<bool>>> MarkAsRead(int notificationId, string userId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{notificationId}")]
        public async Task<ActionResult<APIOperationResponse<bool>>> DeleteNotification(int notificationId)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("send")]
        public async Task<ActionResult<APIOperationResponse<bool>>> SendNotification([FromBody] SendNotificationDto dto)
        {
            var result = await _notificationService.SendNotificationAsync(
                dto.Message,
                dto.Type,
                dto.NotifiableID,
                dto.RecieversIds
            );
            return StatusCode(result.StatusCode, result);
        }
    }
}