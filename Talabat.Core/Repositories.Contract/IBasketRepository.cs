using Talabat.Core.Entities;

namespace Talabat.Core.Repositories.Contract
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> GetCustomerBasketAsync(string basketId);
        Task<CustomerBasket?> CreateOrUpdateCustmerBasketAsync(CustomerBasket basket);
        Task<bool> DeleteCustomerBasketAsync(string basketId);
    }
}
