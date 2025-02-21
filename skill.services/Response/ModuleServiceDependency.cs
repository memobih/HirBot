//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Project.Services.Interfaces;
//using skill.services.Implementation;
//using skill.services.Interfaces;
//using User.Services.Implemntation;

//namespace skill.services.Response
//{
//    public static class ModuleServiceDependency
//    {
//        public static IServiceCollection AddSkillServices(this IServiceCollection service)
//        {
//            service.AddScoped<ISkillService, SkillService>();
//            service.AddScoped<IAuthenticationService, AuthenticationService>();

//            return service;
//        }
//    }
//}