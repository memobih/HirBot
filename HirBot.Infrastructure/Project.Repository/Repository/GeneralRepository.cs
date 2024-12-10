using HirBot.Data.IGenericRepository_IUOW;
using HirBot.EntityFramework.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HirBot.Repository.Repository
{
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class
    {
        #region fields0
        protected ApplicationDbContext _context;
        DbSet<T> _entity;
        #endregion

        #region ctor
        public GeneralRepository(ApplicationDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }
        #endregion

        #region Add entity async
        public async Task<T> AddAsync(T entity)
        {
            await _entity.AddAsync(entity);
            return entity;
        }
        #endregion

        #region Get all entities async
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }
        #endregion

        #region  Get entity by ID async
        public async Task<T> GetByIdAsync(int Id)
        {
            return await _entity.FindAsync(Id);
        }
        #endregion

        #region  Update entity async
        public async Task<T> UpdateAsync(T entity)
        {
            _entity.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }
        #endregion

        #region Delete entity async
        public async Task DeleteAsync(T entity)
        {
            _entity.Remove(entity);
        }
        #endregion

        #region GetLastOrDefaultAsync
        public async Task<T> GetLastOrDefaultAsync<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            return await _entity.OrderByDescending(keySelector).FirstOrDefaultAsync();

        }
        #endregion

        #region GetEntityByPropertyWithInclude
        public async Task<T> GetEntityByPropertyWithIncludeAsync(Expression<Func<T, bool>> attributeSelector, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.SingleOrDefaultAsync(attributeSelector);
        }
        #endregion
    }
}
