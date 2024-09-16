namespace Talabat.Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket(string id)
        {
            Id = id;
            Items = new List<BasketItems>();
        }

        public string Id { get; set; }
        public List<BasketItems> Items { get; set; }
    }
}
