using Microsoft.Extensions.DependencyInjection;
using HirBot.Repository.Repository;
using Project.Repository.Repository;
using HirBot.Data.IGenericRepository_IUOW;
using HirBot.Data.Interfaces;
namespace HirBot.Repository
{
    public static class ModuleInfrastructureDependences
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection service)
        {
            service.AddTransient<UnitOfWork>();
            service.AddScoped<IImageHandler, ImageHandler>();
            return service;
        }
    }
}
