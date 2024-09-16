namespace Talabat.Core.Entities.Identity
{
    public class Address
    {
        public int Id { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string AppUserId { get; set; }
    }
}