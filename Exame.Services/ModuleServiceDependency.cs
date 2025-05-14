using Exame.Services.Implemntation;
using Exame.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services
{
    public static class ModuleServiceDependency
    {
        public static IServiceCollection AddExameServices(this IServiceCollection service)
        {
            service.AddTransient<IExameService , ExameService>();
            service.AddTransient<IDailyChanalgeService , DailyChanalgeService>();
            service.AddTransient<IQuestionService, QuestionService>();
            return service;
        }
    }
}
