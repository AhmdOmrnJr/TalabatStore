using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories.Contract
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
         Task<T?> GetByIdAsync(int id);
         Task<IReadOnlyList<T>> GetAllAsync();

        Task<T?> GetWithSpecsAsync(ISpecifications<T> specs);
        Task<IReadOnlyList<T>> GetAllWithSpecsAsync(ISpecifications<T> specs);
        Task<int> GetCountAsync(ISpecifications<T> specs);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
