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
        Task<APIOperationResponse<object>> GetAllForUserAsync(DateTime? after = null, int limit = 15, bool? isread = null, NotificationType? type = null, string? search = null);
        Task<APIOperationResponse<bool>> MarkAsReadAsync(List<int>Notificationsids);
        Task<APIOperationResponse<bool>> DeleteNotificationAsync(int notificationId);
        Task<APIOperationResponse<bool>> SendNotificationAsync(string message, NotificationType type, NotficationStatus status , string referenceId, List<string> RecieversIds);
        Task<APIOperationResponse<int>> CountUnreadNotificationsAsync();
    }
}