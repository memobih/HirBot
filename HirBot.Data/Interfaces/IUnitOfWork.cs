
using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;

namespace HirBot.Data.IGenericRepository_IUOW
{
    public interface IUnitOfWork : IDisposable
    {

        public IGeneralRepository<Company> Companies {  get; }
        public IGeneralRepository<ApplicationUser> Users { get; }
        public IGeneralRepository<Education> Educations { get; } 
        public IGeneralRepository<Experience> Experiences { get; } 
        public IGeneralRepository <Job> Jobs { get; } 
        public IGeneralRepository<Skill> Skills { get; }
        public IGeneralRepository<Level> Levels { get; }
        public IGeneralRepository<Application> Applications { get;  }
        Task<bool> SaveAsync();
    }
}
