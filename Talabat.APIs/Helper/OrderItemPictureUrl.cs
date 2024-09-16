using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Helper
{
    public class OrderItemPictureUrl : IValueResolver<OrderItems, OrderItemsDto, string>
    {
        private readonly IConfiguration _configuration;

        public OrderItemPictureUrl(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(OrderItems source, OrderItemsDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.ProductUrl))
                return $"{_configuration["BaseUrl"]}/{source.Product.ProductUrl}";

            return string.Empty;
        }
    }
}
