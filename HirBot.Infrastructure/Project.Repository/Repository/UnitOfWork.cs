﻿using HirBot.Data.IGenericRepository_IUOW;
using HirBot.EntityFramework.DataBaseContext;
using HirBot.Repository.Repository;
using HirBot.Data.Entities;
using HirBot.Comman.Idenitity;

namespace Project.Repository.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationDbContext _context;

        public IGeneralRepository<ApplicationUser> Users { get; private set; }
        private IGeneralRepository<RefreshToken> refreshs { get; set; }
        public IGeneralRepository<Company> Companies { get;   private set ;} 
        public IGeneralRepository<Education> Educations { get; private set; }
         
        public IGeneralRepository<Experience> Experiences { get; private set; } 

        public IGeneralRepository<Job> Jobs { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            refreshs =new GeneralRepository<RefreshToken> (_context);
            Users = new GeneralRepository<ApplicationUser>(_context);
            Companies=new GeneralRepository<Company>(_context);
            Educations=new GeneralRepository<Education>(_context); 
            Experiences =new GeneralRepository<Experience>(_context); 
            Jobs=new GeneralRepository<Job>(_context);
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