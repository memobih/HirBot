using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Enums;

namespace Notification.Services.DataTransferObjects
{
    public class SendNotificationDto
    {
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string NotifiableID { get; set; }
        public List<string> RecieversIds { get; set; }
    }
}