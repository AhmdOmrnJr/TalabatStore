using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories.Contract
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
         Task<T?> GetAsync(int id);
         Task<IReadOnlyList<T>> GetAllAsync();

        Task<T?> GetWithSpecsAsync(ISpecifications<T> specs);
        Task<IReadOnlyList<T>> GetAllWithSpecsAsync(ISpecifications<T> specs);
    }
}
