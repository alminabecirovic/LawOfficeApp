using System.Collections.Generic;
using System.Threading.Tasks;

namespace LawOfficeApp.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
    }
}