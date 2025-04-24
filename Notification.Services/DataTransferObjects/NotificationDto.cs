using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Enums;

namespace Notification.Services.DataTransferObjects
{
    public class NotificationDto
    {
         public string ID { get; set; }
        public string? Message { get; set; }
        public string? UserID { get; set; }
        public NotificationType Type { get; set; }
        public string ReferenceID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}