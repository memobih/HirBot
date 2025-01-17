﻿using Mailing;
using Microsoft.Extensions.DependencyInjection;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.Implemntation;

namespace User.Services
{
    public static class ModuleServicesDependences
    {
        public static IServiceCollection AddUsersServices(this IServiceCollection service)
        {
            service.AddTransient<IAuthenticationService, AuthenticationService>();
            service.AddScoped<IMailingService, MailingService>();
            return service;
        }
    }
}
