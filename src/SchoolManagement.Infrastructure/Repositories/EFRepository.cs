using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T>
    where T : class
{
    protected readonly SchoolDbContext _context;
    private readonly DbSet<T> _dbSet;

    public EfRepository(SchoolDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbSet.AsQueryable();
        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);
}
