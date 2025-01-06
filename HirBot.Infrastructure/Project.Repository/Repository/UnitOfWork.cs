using HirBot.Data.IGenericRepository_IUOW;
using HirBot.EntityFramework.DataBaseContext;
using HirBot.Comman.Idenitity;
using HirBot.Repository.Repository;
using HirBot.Data.Entities;

namespace Project.Repository.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGeneralRepository<User> Users { get; private set; }
        private IGeneralRepository<RefreshToken> refreshs { get; set; }
        public IGeneralRepository<Company> Companies { get;   private set ;}

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            refreshs =new GeneralRepository<RefreshToken> (_context);
            Users = new GeneralRepository<User>(_context);
            Companies=new GeneralRepository<Company>(_context);
        }
        public async Task<bool> SaveAsync()
        {
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}