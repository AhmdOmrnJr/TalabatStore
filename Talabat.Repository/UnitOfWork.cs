using System.Collections;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dpContext;
        private Hashtable _repositories;

        public UnitOfWork(StoreContext dpContext)
        {
            _dpContext = dpContext;
            _repositories = new Hashtable();
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<TEntity>(_dpContext);
                _repositories.Add(key, repository);
            }
            return _repositories[key] as IGenericRepository<TEntity>;
        }

        public Task<int> CompleteAsync()
            => _dpContext.SaveChangesAsync();

        public ValueTask DisposeAsync()
            => _dpContext.DisposeAsync();
    }
}
