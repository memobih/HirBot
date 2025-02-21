using Microsoft.Extensions.DependencyInjection;
using skill.services.Implementation;
using skill.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace skill.services.Response
{
    public static class ModuleServiceDependency
    {
        public static IServiceCollection AddSkillServices(this IServiceCollection service)
        {
            service.AddScoped<ISkillService, SkillService>();

            return service;
        }
    }
}