using System.Collections.Generic;
using System.Threading.Tasks;
using LawOfficeApp.Data;
using Microsoft.EntityFrameworkCore;

namespace LawOfficeApp.Repositories
{
    
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly LawOfficeDbContext _context;
        protected readonly DbSet<T> _dbSet;

        // Konstruktor
        public Repository(LawOfficeDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); 
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}