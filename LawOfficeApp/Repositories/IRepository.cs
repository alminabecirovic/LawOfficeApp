using System.Collections.Generic;
using System.Threading.Tasks;

namespace LawOfficeApp.Repositories
{
    /// <summary>
    /// ✅ GENERIČKI INTERFEJS - Repository pattern
    /// Demonstrira generic constraint: where T : class
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
    }
}