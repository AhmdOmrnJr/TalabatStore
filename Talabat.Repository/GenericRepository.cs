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

        public async Task<T?> GetAsync(int id)
            => await _dbContext.Set<T>().FindAsync(id);

        public async Task<T?> GetWithSpecsAsync(ISpecifications<T> specs)
            => await SpesificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<T>> GetAllWithSpecsAsync(ISpecifications<T> specs)
            => await SpesificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs).ToListAsync();

        //private IQueryable<T> ApplySpecifications(ISpecifications<T> specs)
        //    => SpesificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs);
    }
}
