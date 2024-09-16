using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Dtos
{
    public class OrderToReturnDto
    {
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } 
        public string Status { get; set; }
        public Address ShippingAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public decimal DeliveryMethodCost { get; set; }
        public ICollection<OrderItemsDto> Items { get; set; } = new HashSet<OrderItemsDto>();
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
