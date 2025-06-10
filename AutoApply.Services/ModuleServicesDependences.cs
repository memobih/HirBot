using AutoApply.Services.Implemntations;
using AutoApply.Services.Interfaces;
using Exame.Services.Implemntation;
using Exame.Services.Interfaces;
using Jop.Services.Implemntations;
using Jop.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoApply.Services
{
    public static class ModuleServicesDependences
    {
        public static IServiceCollection AddAutoApplyServices(this IServiceCollection service)
        {
            service.AddTransient<IAutoApplyService, AutoApplyService>();

            return service;
        }
    }
}
