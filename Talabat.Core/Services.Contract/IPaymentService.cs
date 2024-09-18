using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Services.Contract
{
    public interface IPaymentService
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order> UpdatePaymentIntentToSucceeddedOrFailed(string paymentIntentId, bool isSucceedded);
    }
}
