using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Enums;

namespace Notification.Services.DataTransferObjects
{
    public class NotificationDto
    {
         public int  ID { get; set; }
        public int notification_id { get; set; }
        public Type type { get; set; } = new Type();
        public string? Message { get; set; }
   
        public DateTime  Created_at { get; set; }
        public bool Is_read{ get; set; }
        public DateTime ? read_at {  get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
    public class Type
    {
        public NotficationStatus  ? action { get; set; }
        public NotificationType category { get; set; } 
        public string label { get; set; }

    }

}