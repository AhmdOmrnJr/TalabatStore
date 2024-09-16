using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderSpecifications : BaseSpecifications<Order>
    {
        public OrderSpecifications(string BuyerEmail)
            : base(order => order.BuyerEmail == BuyerEmail)
        {
            Includes.Add(order => order.DeliveryMethod);
            Includes.Add(order => order.Items);
            AddOrderByDesc(order => order.OrderDate);

            //ApplyPagination((PageIndex - 1) * PageSize, PageSize);
        }

        public OrderSpecifications(int id, string BuyerEmail)
            : base(order => order.BuyerEmail == BuyerEmail && order.Id == id)
        {
            Includes.Add(order => order.DeliveryMethod);
            Includes.Add(order => order.Items);
        }
    }
}
