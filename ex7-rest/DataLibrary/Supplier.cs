namespace DataLibrary
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public required string SupplierName { get; set; }
        public required string ContactName { get; set; }
        public string? Phone { get; set; }
    }
}
