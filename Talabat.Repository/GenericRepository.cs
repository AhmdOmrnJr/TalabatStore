using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _dbContext.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(int id)
            => await _dbContext.Set<T>().FindAsync(id);

        public async Task<T?> GetWithSpecsAsync(ISpecifications<T> specs)
            => await SpesificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<T>> GetAllWithSpecsAsync(ISpecifications<T> specs)
            => await SpesificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs).ToListAsync();

        public async Task<int> GetCountAsync(ISpecifications<T> specs)
            => await SpesificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs).CountAsync();

        public async Task AddAsync(T entity)
            => await _dbContext.AddAsync(entity);
        //=> await _dbContext.Set<T>().AddAsync(entity);  // both are correct || this one from entityframework 6.0

        public void Update(T entity)
            => _dbContext.Update(entity);

        public void Delete(T entity)
            => _dbContext.Remove(entity);

        //private IQueryable<T> ApplySpecifications(ISpecifications<T> specs)
        //    => SpesificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs);
    }
}
