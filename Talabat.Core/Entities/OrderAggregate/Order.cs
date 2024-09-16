using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.OrderAggregate
{
    public class Order : BaseEntity
    {
        public Order()
        {
            
        }
        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItems> items, decimal subTotal)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }
        public int? DeliveryMethodId { get; set; } // make it nulluble or change it in the Migration file and the Snapshot File
        public DeliveryMethod DeliveryMethod { get; set; }
        public ICollection<OrderItems> Items { get; set; } = new HashSet<OrderItems>();
        public decimal SubTotal { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;

        //[NotMapped]
        //public decimal Total => SubTotal + DeliveryMethod.Cost;

        public decimal GetTotal()
            => SubTotal + DeliveryMethod.Cost;
    }
}
