using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RadarService.Data.Repositories
{
    public class Repository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;

        }
        public async Task AddAsync(TEntity entity) => await _context.Set<TEntity>().AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities) => await _context.Set<TEntity>().AddRangeAsync(entities);

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression) => await _context.Set<TEntity>().AnyAsync(expression);

        public IQueryable<TEntity> GetAll() => _context.Set<TEntity>().AsNoTracking();

        public async Task<TEntity> GetByIdAsync(int id) => await _context.Set<TEntity>().FindAsync(id);

        public void Remove(TEntity entity) => _context.Set<TEntity>().Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().RemoveRange(entities);

        public void Update(TEntity entity) => _context.Set<TEntity>().Update(entity);

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression) => _context.Set<TEntity>().Where(expression);

        public async Task<TEntity?> GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate) => await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateRange(IEnumerable<TEntity> entity) => _context.Set<TEntity>().UpdateRange(entity);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
