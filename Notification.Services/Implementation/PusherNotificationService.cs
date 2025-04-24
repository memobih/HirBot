using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PusherServer;

namespace Notification.Services.Implementation
{
    public class PusherNotificationService
    {
        private readonly Pusher _pusher;

        public PusherNotificationService(IConfiguration config)
        {
            var options = new PusherOptions
            {
                Cluster = config["Pusher:Cluster"],
                Encrypted = true
            };

            _pusher = new Pusher(
                config["Pusher:AppId"],
                config["Pusher:Key"],
                config["Pusher:Secret"],
                options
            );
        }

        public async Task TriggerNotificationAsync(string userId, string eventName, object data)
        {
            var channel = $"user-{userId}";
                var result = await _pusher.TriggerAsync(channel, eventName, data);
                
            

        }
    }
}