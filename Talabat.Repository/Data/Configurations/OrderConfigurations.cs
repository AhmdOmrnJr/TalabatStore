using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Repository.Data.Configurations
{
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(order => order.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner());

            builder.Property(order => order.Status)
                .HasConversion(

                    orderStatus => orderStatus.ToString(),

                    orderStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus)

                    );

            builder.Property(order => order.SubTotal).HasColumnType("decimal(18,2)");

            builder.HasOne(order => order.DeliveryMethod).WithMany().OnDelete(DeleteBehavior.SetNull);
        }
    }
}
