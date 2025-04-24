using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Notification.Services.Implementation;
using Notification.Services.Interfaces;

namespace Notification.Services
{
    public static class ModuleServicesDependences
    {
        public static IServiceCollection AddNotificationService(this IServiceCollection service)
        {
            service.AddTransient<INotificationService, NotificationService>();
            service.AddScoped<PusherNotificationService>();

            return service;
        }
    }
}
