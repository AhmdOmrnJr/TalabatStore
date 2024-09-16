namespace Talabat.Core.Entities.OrderAggregate
{
    public class OrderItemOrdered
    {
        public OrderItemOrdered()
        {
            
        }
        public OrderItemOrdered(int productId, string productName, string productUrl)
        {
            ProductId = productId;
            ProductName = productName;
            ProductUrl = productUrl;
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
    }
}