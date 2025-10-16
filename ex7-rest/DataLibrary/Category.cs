namespace DataLibrary
{
    public class Category
    {
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int SupplierId { get; set; }
        public required Supplier Supplier { get; set; }
    }
}
