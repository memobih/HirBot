﻿
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
        Task<bool> SaveAsync();
    }
}
