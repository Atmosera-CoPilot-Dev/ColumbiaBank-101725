namespace DataLibrary
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
