using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Microsoft.EntityFrameworkCore;
using Notification.Services.DataTransferObjects;
using Notification.Services.Interfaces;
using Project.Repository.Repository;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly PusherNotificationService _pusher;
        private readonly UnitOfWork _context;

        public NotificationService(PusherNotificationService pusherNotificationService, UnitOfWork context)
        {
            _context = context;
            _pusher = pusherNotificationService;
        }

        public async Task<APIOperationResponse<int>> CountUnreadNotificationsAsync(string userId)
        {
            var count = await _context._context.NotificationRecivers
                .CountAsync(n => n.ReciverID == userId && !n.read_at.HasValue);

            return APIOperationResponse<int>.Success(count);
        }

        public async Task<APIOperationResponse<bool>> DeleteNotificationAsync(int notificationId)
        {
            var noti = await _context._context.Notifications.FindAsync(notificationId);
            if (noti == null)
                return APIOperationResponse<bool>.NotFound("Notification not found");

            var notificationRecivers = await _context._context.NotificationRecivers
                .Where(n => n.NotificationID == notificationId)
                .ToListAsync();

            _context._context.NotificationRecivers.RemoveRange(notificationRecivers);
            _context._context.Notifications.Remove(noti);

            try
            {
                await _context._context.SaveChangesAsync();
                return APIOperationResponse<bool>.Deleted("Notification deleted successfully");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<bool>.ServerError("Error deleting notification", new List<string> { ex.Message });
            }
        }

        public async Task<APIOperationResponse<List<NotificationDto>>> GetAllForUserAsync(string userId)
{
    var user = await _context._context.Users.FindAsync(userId);
    if (user == null)
        return APIOperationResponse<List<NotificationDto>>.NotFound("User not found");

    var notificationReceivers = await _context._context.NotificationRecivers
        .Where(n => n.ReciverID == userId)
        .Include(n => n.Notification)
        .ToListAsync();

    var notificationDtos = new List<NotificationDto>();

    foreach (var receiver in notificationReceivers)
    {
        var notification = receiver.Notification;
        var dto = new NotificationDto
        {
            ID = notification.ID.ToString(),
            Message = notification.massage,
            UserID = receiver.ReciverID,
            Type = notification.Notifiable_Type,
            ReferenceID = notification.Notifiable_ID,
            CreatedAt = notification.CreationDate,
            IsRead = receiver.read_at.HasValue,
            Metadata = new Dictionary<string, object>()
        };

        // 🔁 Enrich Metadata based on notification type
        switch (notification.Notifiable_Type)
        {
            case NotificationType.job:
                var job = await _context._context.Jobs
                    .Where(j => j.ID.ToString() == notification.Notifiable_ID)
                    .Select(j => new { j.ID, j.Title, j.CreationDate,j.Company})
                    
                    .FirstOrDefaultAsync();
                if (job != null)
                {
                    dto.Metadata["JobTitle"] = job.Title;
                    dto.Metadata["PostedDate"] = job.CreationDate;
                }
                break;

            case NotificationType.Interview:
                var interview = await _context._context.Interviews
                    .Where(i => i.ID.ToString() == notification.Notifiable_ID)
                    .Select(i => new { i.ID, i.CandidateName, i.StartTime ,i.Mode,i.Application})
                    .Include(i => i.Application)
                    .ThenInclude(a => a.Job)
                    .ThenInclude(j => j.Company)
                    .FirstOrDefaultAsync();
                if (interview != null)
                {
                    
                    dto.Metadata["Date"] = interview.StartTime;
                    dto.Metadata["Mode"] = interview.Mode;
                    dto.Metadata["CompanyLogo"] = interview.Application.Job.Company.Logo??string.Empty;
                    dto.Metadata["CompanyName"] = interview.Application.Job.Company.Name??string.Empty;
                    dto.Metadata["JobTitle"] = interview.Application.Job.Title??string.Empty;

                }
                break;

            case NotificationType.post:
                // Add logic here for post enrichment if needed
                break;
            case NotificationType.Application:
                var application = await _context._context.Applications
                    .Where(a => a.ID.ToString() == notification.Notifiable_ID)
                    .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                    .FirstOrDefaultAsync();
                if (application != null)
                {
                    dto.Metadata["CompanyLogo"] = application.Job.Company.Logo??string.Empty;
                    dto.Metadata["CompanyName"] = application.Job.Company.Name??string.Empty;
                    dto.Metadata["JobTitle"] = application.Job.Title??string.Empty;
                    dto.Metadata["ApplicationStatus"] = application.status.ToString();
                }
                break;

            // Add more types as needed...
        }

        notificationDtos.Add(dto);
    }

    if (!notificationDtos.Any())
        return APIOperationResponse<List<NotificationDto>>.NotFound("No notifications found for this user");

    return APIOperationResponse<List<NotificationDto>>.Success(notificationDtos);
}


        public async Task<APIOperationResponse<bool>> MarkAsReadAsync(int notificationId, string userId)
        {
            var receiver = await _context._context.NotificationRecivers
                .FirstOrDefaultAsync(n => n.NotificationID == notificationId && n.ReciverID == userId);
            if (receiver == null || receiver.read_at.HasValue)
                return APIOperationResponse<bool>.NotFound("Notification receiver not found or already read");

            receiver.read_at = DateTime.UtcNow;
            try
            {
                await _context._context.SaveChangesAsync();
                return APIOperationResponse<bool>.Success(true, "Notification marked as read");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<bool>.ServerError("Error marking notification as read", new List<string> { ex.Message });
            }
        }

        public async Task<APIOperationResponse<bool>> SendNotificationAsync(string message, NotificationType type, string referenceId, List<string> userIds)
        {
            var notification = new HirBot.Data.Entities.Notification
            {
                massage = message,
                Notifiable_Type = type,
                Notifiable_ID = referenceId,
                CreationDate = DateTime.UtcNow
            };

            await _context._context.Notifications.AddAsync(notification);
            try
            {
                await _context._context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return APIOperationResponse<bool>.ServerError("Error saving notification", new List<string> { ex.Message });
            }

            foreach (var userId in userIds)
            {
                var receiver = new NotificationReciver
                {
                    ReciverID = userId,
                    NotificationID = notification.ID,
                    read_at = null,
                    CreationDate = DateTime.UtcNow,
                };
                await _context._context.NotificationRecivers.AddAsync(receiver);

                await _pusher.TriggerNotificationAsync($"{userId}", "new-notification", new
                {
                    message,
                    type,
                    referenceId,
                    notificationId = notification.ID,
                });
            }

            try
            {
                await _context._context.SaveChangesAsync();
                return APIOperationResponse<bool>.Success(true, "Notification sent successfully");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<bool>.ServerError("Error saving notification receivers", new List<string> { ex.Message });
            }
        }
    }
}
