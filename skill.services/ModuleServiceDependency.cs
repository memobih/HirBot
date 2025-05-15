using HirBot.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Project.Repository.Repository;
using skill.services.Implementation;
using skill.services.Interfaces;
using Skill.services.Implementation;
using Skill.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace skill.services
{
    public static class ModuleServiceDependency
    {
        public static IServiceCollection AddSkillServices(this IServiceCollection service)
        {
            service.AddScoped<ISkillService, SkillService>();
            service.AddScoped<IImageHandler, ImageHandler>();
            service.AddScoped<ILevelService , LevelService>();


            return service;
        }
    }
}