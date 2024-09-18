using Microsoft.Extensions.Configuration;
using Stripe;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration, IBasketRepository basketRepo, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];

            var basket = await _basketRepo.GetCustomerBasketAsync(basketId);

            if (basket is null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                basket.ShippingPrice = deliveryMethod.Cost;
                shippingPrice = deliveryMethod.Cost;

            }

            if (basket?.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if (item.Price != product.Price)
                        item.Price = product.Price;
                }

                PaymentIntentService paymentIntentService = new PaymentIntentService();

                PaymentIntent paymentIntent;

                if (string.IsNullOrEmpty(basket.PaymentIntentId))
                {
                    var createOptions = new PaymentIntentCreateOptions()
                    {
                        Amount = (long) basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long) shippingPrice * 100,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string>() { "card" }
                    };

                    paymentIntent = await paymentIntentService.CreateAsync(createOptions);

                    basket.PaymentIntentId = paymentIntent.Id;
                    basket.ClientSecret = paymentIntent.ClientSecret;
                }
                else
                {
                    var updatedOptions = new PaymentIntentUpdateOptions()
                    {
                        Amount = (long)basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long)shippingPrice * 100,
                    };

                    await paymentIntentService.UpdateAsync(basket.PaymentIntentId, updatedOptions);
                }

                await _basketRepo.CreateOrUpdateCustmerBasketAsync(basket);
            }

            return basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceeddedOrFailed(string paymentIntentId, bool isSucceedded)
        {
            var specs = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetWithSpecsAsync(specs);

            if (isSucceedded)
                order.Status = OrderStatus.PaymentReceived;

            else
                order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.CompleteAsync();

            return order;
        }
    }
}
