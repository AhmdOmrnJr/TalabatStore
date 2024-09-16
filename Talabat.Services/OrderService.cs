using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;

namespace Talabat.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IBasketRepository basketRepo, IUnitOfWork unitOfWork)
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var basket = await _basketRepo.GetCustomerBasketAsync(basketId);

            var orderItems = new List<OrderItems>();

            var productRepo = _unitOfWork.Repository<Product>();

            if (basket?.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await productRepo.GetByIdAsync(item.Id);
                    var productItemOrdred = new OrderItemOrdered(item.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItems(productItemOrdred, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
            }

            var subTotal = orderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal);

            await _unitOfWork.Repository<Order>().AddAsync(order);

            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return null;

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderSpecifications(buyerEmail);
            var orders = await orderRepo.GetAllWithSpecsAsync(spec);
            return orders;
        }

        public async Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderSpecifications(orderId, buyerEmail);
            var order = await orderRepo.GetWithSpecsAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        //{
        //    var deliveryMethodsRepo = _unitOfWork.Repository<DeliveryMethod>();
        //    var deliveryMethods = await deliveryMethodsRepo.GetAllAsync();
        //    return deliveryMethods;
        //}
    }
}
