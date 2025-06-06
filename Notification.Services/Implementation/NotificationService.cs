
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Notification.Services.DataTransferObjects;
using Notification.Services.Interfaces;
using Project.Repository.Repository;
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

        public async Task<APIOperationResponse<int>> CountUnreadNotificationsAsync()
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null)
                return APIOperationResponse<int>.UnOthrized("Unauthorized access");
            var count = await _context._context.NotificationRecivers
                .CountAsync(n => n.ReciverID == user.Id && !n.read_at.HasValue);

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

        public async Task<APIOperationResponse<object>> GetAllForUserAsync(DateTime? after = null, int limit = 15, bool? isread = null, List<NotificationType>? type = null, string? search = null)
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null)
                return APIOperationResponse<object>.UnOthrized("Unauthorized access");
            var userId = user.Id;
            var notificationReceivers = await _context._context.NotificationRecivers
                .Where(n => n.ReciverID == userId)
                .Include(n => n.Notification)
                .ToListAsync();

            var notificationDtos = new List<NotificationDto>();
            var unread_count = new Dictionary<string, int>();
            unread_count.Add("interview", 0);
            unread_count.Add("job", 0);
            unread_count.Add("application", 0);




            foreach (var receiver in notificationReceivers)
            {
                var notification = receiver.Notification;
                if (notification == null || notification.Notifiable_Type > NotificationType.Application)
                    continue;


                var dto = new NotificationDto
                {
                    ID = receiver.ID,
                    notification_id = notification.ID,
                    Message = notification.massage,

                    Created_at = notification.CreationDate,
                    Is_read = receiver.read_at.HasValue,
                    Metadata = new Dictionary<string, object>()
                };
                dto.type.category = notification.Notifiable_Type;
                dto.type.label = notification.Notifiable_Type + " " + notification.type;
                dto.type.action = notification.type;

                switch (notification.Notifiable_Type)
                {
                    case NotificationType.job:
                        if (dto.Is_read == false)
                            unread_count["job"]++;
                        if (int.TryParse(notification.Notifiable_ID, out int jobId))
                        {
                            var job = await _context._context.Jobs
                                .Where(j => j.ID == jobId)
                                .Include(j => j.Company)
                                .FirstOrDefaultAsync();

                            if (job != null)
                            {

                                dto.Metadata["job"] =
                                    new
                                    {
                                        job = new
                                        {
                                            id = jobId,
                                            PostedDate = job.CreationDate,
                                            CompanyLogo = job.Company?.Logo,
                                            CompanyName = job.Company?.Name,


                                        }
                                    };
                            }
                        }
                        break;

                    case NotificationType.Interview:
                        if (dto.Is_read == false)
                            unread_count["interview"]++;

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
                                dto.Metadata["interview"] = new
                                {
                                    interview = new
                                    {
                                        id = interview.ID,
                                        interview.StartTime,
                                        interview.Mode,
                                        CompanyLogo =
                                        interview.Application.Job.Company.Logo,
                                        CompanyName = interview.Application.Job.Company.Name,
                                        jobTitle = interview.Application.Job.Title
                                    }
                                };
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
                                if (dto.Is_read == false)
                                    unread_count["application"]++;
                                dto.Metadata["application"] = new
                                {
                                    application = new
                                    {
                                        id = application.ID,
                                        CompanyLogo = application.Job.Company.Logo,
                                        CompanyName = application.Job.Company.Name,
                                        JobTitle = application.Job.Title,
                                        ApplicationStatus = application.status,
                                    }
                                };
                            }
                        }
                        break;
                    default:
                        break;
                }

                notificationDtos.Add(dto);
            }

            filter(ref notificationDtos, after, limit, isread, type, search);


            DateTime? nextPageCursor = null;
            if (notificationDtos.Count != 0) nextPageCursor = notificationDtos.Last().Created_at;
            return APIOperationResponse<object>.Success(new
            {
                nextPageCursor = nextPageCursor,
                hasMore = (notificationDtos.Count > limit)
                ,
                pageSize = limit,
                totalRecords = notificationDtos.Count()
                ,
                unread_count
                ,
                data = Paginate(notificationDtos, 1, limit
                ),




            });

        }


        public async Task<APIOperationResponse<bool>> MarkAsReadAsync(List<int> notificationsids)
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            if (user == null)
                return APIOperationResponse<bool>.UnOthrized("Unauthorized access");

            var notificationRecivers = await _context._context.NotificationRecivers
                .Where(n => n.ReciverID == user.Id && notificationsids.Contains(n.ID))
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

        public async Task<APIOperationResponse<bool>> SendNotificationAsync(string message, NotificationType type, NotficationStatus status, string referenceId, List<string> userIds, object data)
        {
            string typ = string.Empty;
            typ = type.ToString();
            var notification = new HirBot.Data.Entities.Notification
            {
                massage = message,
                Notifiable_Type = type,
                Notifiable_ID = referenceId,
                CreationDate = DateTime.UtcNow,
                type = status
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
                var flatData = new Dictionary<string, object>
                        {
                         { "id", notification.ID },
                          { "notification_id", notification.ID }
                                                    };
                foreach (var prop in data.GetType().GetProperties())
{
    flatData[prop.Name] = prop.GetValue(data);
}
                await _pusher.TriggerNotificationAsync($"{userId}", "new.notification", flatData);
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
        #region 
        private void filter(ref List<NotificationDto> notifications, DateTime? after = null, int limit = 15, bool? isread = null, List<NotificationType>? type = null, string? search = null)
        {
            if (after != null)
                notifications = notifications.Where(n => n.Created_at < after).ToList();
            if (isread != null)
                notifications = notifications.Where(n => n.Is_read == isread).ToList();
            if (type != null)
                notifications = notifications.Where(n => type.Contains(n.type.category)).ToList();
            if (search != null)
                notifications = notifications.Where(n => n.Message.Contains(search)).ToList();
            notifications = notifications.OrderByDescending(n => n.Created_at).ToList();

        }


        private List<T> Paginate<T>(List<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        #endregion
    }
}
