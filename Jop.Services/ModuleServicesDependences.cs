using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jop.Services.Implemntations;
using Jop.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Jop.Services
{
    public static class ModuleServicesDependences
    {
        public static IServiceCollection AddJopServices(this IServiceCollection service)
        {
            service.AddTransient<IJobService, JobServices>();
            service.AddTransient<IApplicationService, ApplicationService>();
            service.AddTransient<IEmployeeService, EmployeeService>();
            service.AddTransient<IInterviewService, InterviewService>();

            return service; 
        }
    }
}
