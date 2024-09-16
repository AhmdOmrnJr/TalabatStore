using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using UserAddress = Talabat.Core.Entities.Identity.Address;
using OrderAddress = Talabat.Core.Entities.OrderAggregate.Address;

namespace Talabat.APIs.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, o => o.MapFrom(src => src.Brand.Name))
                .ForMember(d => d.Category, o => o.MapFrom(src => src.Category.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasket, CustomerBasketDto>();
            CreateMap<BasketItems, BasketItemsDto>();

            CreateMap<AddressDto, OrderAddress>();

            CreateMap<UserAddress, AddressDto>();
            CreateMap<AddressDto, UserAddress>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(src => src.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(src => src.DeliveryMethod.Cost));

            CreateMap<OrderItems, OrderItemsDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(src => src.Product.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(src => src.Product.ProductName))
                .ForMember(d => d.ProductUrl, o => o.MapFrom(src => src.Product.ProductUrl))
                .ForMember(d => d.ProductUrl, o => o.MapFrom<OrderItemPictureUrl>());
        }
    }
}
