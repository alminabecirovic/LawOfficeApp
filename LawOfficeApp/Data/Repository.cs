using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawOfficeApp.Data;

public class Repository<T> where T : class
{
    private readonly LawOfficeDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(LawOfficeDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    // Client-side generic filter helper (use IQueryable for server-side filtering)
    public async Task<List<T>> WhereAsync(Func<T, bool> predicate)
        => await Task.Run(() => _dbSet.AsEnumerable().Where(predicate).ToList());
}
