using Microsoft.Extensions.DependencyInjection;
using HirBot.Repository.Repository;
using Project.Repository.Repository;
namespace HirBot.Repository
{
    public static class ModuleInfrastructureDependences
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection service)
        {
            service.AddTransient<UnitOfWork>();
            return service;
        }
    }
}
