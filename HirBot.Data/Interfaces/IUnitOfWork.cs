using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;

namespace HirBot.Data.IGenericRepository_IUOW
{
    public interface IUnitOfWork : IDisposable
    {

        public IGeneralRepository<User> Users { get; }
        public IGeneralRepository<Company> Companies {  get; }
        Task<bool> SaveAsync();
    }
}
