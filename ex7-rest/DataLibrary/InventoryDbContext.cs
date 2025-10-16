using Microsoft.EntityFrameworkCore;

namespace DataLibrary
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map Supplier entity to suppliers table
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");
                entity.HasKey(s => s.SupplierId);
                entity.Property(s => s.SupplierId).HasColumnName("supplier_id");
                entity.Property(s => s.SupplierName).HasColumnName("supplier_name").IsRequired();
                entity.Property(s => s.ContactName).HasColumnName("contact_name").IsRequired();
                entity.Property(s => s.Phone).HasColumnName("phone");
            });

            // Map Category entity to categories table
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.CategoryId).HasColumnName("category_id");
                entity.Property(c => c.CategoryName).HasColumnName("category_name").IsRequired();
                entity.Property(c => c.SupplierId).HasColumnName("supplier_id").IsRequired();

                // Define foreign key relationship
                entity.HasOne(c => c.Supplier)
                      .WithMany()
                      .HasForeignKey(c => c.SupplierId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Map Product entity to products table
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.ProductId).HasColumnName("product_id");
                entity.Property(p => p.ProductName).HasColumnName("product_name").IsRequired();
                entity.Property(p => p.CategoryId).HasColumnName("category_id").IsRequired();
                entity.Property(p => p.Price).HasColumnName("price").HasColumnType("REAL");
                entity.Property(p => p.Stock).HasColumnName("stock");

                // Define foreign key relationship
                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
