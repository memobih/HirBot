using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Notification.Services.DataTransferObjects;

namespace Notification.Services.Interfaces
{
    public interface INotificationService
    {
        Task<APIOperationResponse<List<NotificationDto>>> GetAllForUserAsync(string userId);
        Task<APIOperationResponse<bool>> MarkAsReadAsync(int notificationId, string userId);
        Task<APIOperationResponse<bool>> DeleteNotificationAsync(int notificationId);
        Task<APIOperationResponse<bool>> SendNotificationAsync(string message, NotificationType type, string referenceId, List<string> RecieversIds);
        Task<APIOperationResponse<int>> CountUnreadNotificationsAsync(string userId);
    }
}