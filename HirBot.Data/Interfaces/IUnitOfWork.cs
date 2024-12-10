using HirBot.Comman.Idenitity;

namespace HirBot.Data.IGenericRepository_IUOW
{
    public interface IUnitOfWork : IDisposable
    {

        public IGeneralRepository<ApplicationUser> Users { get; }
        
        Task<bool> SaveAsync();
    }
}
