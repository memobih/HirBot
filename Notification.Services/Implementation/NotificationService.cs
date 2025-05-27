using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Notification.Services.DataTransferObjects;
using Notification.Services.Interfaces;
using Project.Repository.Repository;
using Project.ResponseHandler.Models;
using Project.Services.Interfaces;
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
        private readonly IAuthenticationService _authenticationService;

        public NotificationService(PusherNotificationService pusherNotificationService, UnitOfWork context, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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
    var tokuser = await _authenticationService.GetCurrentUserAsync();
    if (tokuser == null)
        return APIOperationResponse<List<NotificationDto>>.UnOthrized("Unauthorized access");

    if (tokuser.Id != userId)
        return APIOperationResponse<List<NotificationDto>>.UnOthrized("You are not allowed to access these notifications.");

    var notificationReceivers = await _context._context.NotificationRecivers
        .Where(n => n.ReciverID == userId)
        .Include(n => n.Notification)
        .ToListAsync();

    var notificationDtos = new List<NotificationDto>();

    foreach (var receiver in notificationReceivers)
    {
        var notification = receiver.Notification;
        if (notification == null)
            continue;

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

        switch (notification.Notifiable_Type)
        {
            case NotificationType.job:
                if (int.TryParse(notification.Notifiable_ID, out int jobId))
                {
                    var job = await _context._context.Jobs
                        .Where(j => j.ID == jobId)
                        .Include(j => j.Company)
                        .FirstOrDefaultAsync();

                    if (job != null)
                    {
                        dto.Metadata["JobTitle"] = job.Title;
                        dto.Metadata["PostedDate"] = job.CreationDate;
                        dto.Metadata["CompanyLogo"] = job.Company?.Logo ?? string.Empty;
                        dto.Metadata["CompanyName"] = job.Company?.Name ?? string.Empty;
                    }
                }
                break;

            case NotificationType.Interview:
                if (Guid.TryParse(notification.Notifiable_ID, out Guid interviewId))
                {
                    var interview = await _context._context.Interviews
                        .Where(i => i.ID == interviewId.ToString())
                        .Include(i => i.Application)
                            .ThenInclude(a => a.Job)
                                .ThenInclude(j => j.Company)
                                   .FirstOrDefaultAsync();

                    if (interview != null && interview.Application?.Job?.Company != null)
                    {   
                        dto.Metadata["Date"] = interview.StartTime;
                        dto.Metadata["Mode"] = interview.Mode;
                        dto.Metadata["CompanyLogo"] = interview.Application.Job.Company.Logo ?? string.Empty;
                        dto.Metadata["CompanyName"] = interview.Application.Job.Company.Name ?? string.Empty;
                        dto.Metadata["JobTitle"] = interview.Application.Job.Title ?? string.Empty;
                    }
                }
                break;

            case NotificationType.Application:
                if (int.TryParse(notification.Notifiable_ID, out int appId))
                {
                    var application = await _context._context.Applications
                        .Where(a => a.ID == appId)
                        .Include(a => a.Job)
                            .ThenInclude(j => j.Company)
                        .FirstOrDefaultAsync();

                    if (application != null && application.Job?.Company != null)
                    {
                        dto.Metadata["CompanyLogo"] = application.Job.Company.Logo ?? string.Empty;
                        dto.Metadata["CompanyName"] = application.Job.Company.Name ?? string.Empty;
                        dto.Metadata["JobTitle"] = application.Job.Title ?? string.Empty;
                        dto.Metadata["ApplicationStatus"] = application.status.ToString();
                    }
                }
                break;

            case NotificationType.post:
               
                break;
        }

        dto.Metadata["Source"] = "ASP.NET";
        notificationDtos.Add(dto);
    }

    if (!notificationDtos.Any())
        return APIOperationResponse<List<NotificationDto>>.NotFound("No notifications found for this user");

    return APIOperationResponse<List<NotificationDto>>.Success(notificationDtos);
}

        public async Task<APIOperationResponse<bool>> MarkAsReadAsync(List<string> notificationsids)
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null)
                return APIOperationResponse<bool>.UnOthrized("Unauthorized access");
            
            var notificationRecivers = await _context._context.NotificationRecivers
                .Where(n => n.ReciverID == user.Id && notificationsids.Contains(n.NotificationID.ToString()))
                .ToListAsync();
            if (!notificationRecivers.Any())
                return APIOperationResponse<bool>.NotFound("No notifications found for the given IDs");
            foreach (var receiver in notificationRecivers)
            {
                receiver.read_at = DateTime.UtcNow;
            }
            try
            {
                await _context._context.SaveChangesAsync();
                return APIOperationResponse<bool>.Success(true, "Notifications marked as read successfully");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<bool>.ServerError("Error marking notifications as read", new List<string> { ex.Message });
            }
        }

        public async Task<APIOperationResponse<bool>> SendNotificationAsync(string message, NotificationType type, string referenceId, List<string> userIds)
        {
            string typ = string.Empty;
            if (type == NotificationType.Application)
            {
                typ = "Application";
            }
            else if (type == NotificationType.Interview)
            {
                typ = "Interview";
            }
            else if (type == NotificationType.job)
            {
                typ = "Job";
            }
                var notification = new HirBot.Data.Entities.Notification
                {
                    massage = message,
                    Notifiable_Type= type,
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
                    typ,
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
