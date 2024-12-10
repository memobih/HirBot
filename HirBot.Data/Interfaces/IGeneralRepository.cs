using System.Linq.Expressions;

namespace HirBot.Data.IGenericRepository_IUOW
{
    public interface IGeneralRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int Id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T> GetLastOrDefaultAsync<TKey>(Expression<Func<T, TKey>> keySelector);
        public Task<T> GetEntityByPropertyWithIncludeAsync(Expression<Func<T, bool>> attributeSelector, params Expression<Func<T, object>>[] includes);

    }
}
