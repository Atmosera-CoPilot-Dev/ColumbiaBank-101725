using Microsoft.EntityFrameworkCore; // Add this to ensure the extension method is available
// Ensure the required package is installed: Microsoft.EntityFrameworkCore.InMemory
// You can install it via NuGet Package Manager or the Package Manager Console:
// Install-Package Microsoft.EntityFrameworkCore.InMemory

using DataLibrary; // Required to access InventoryDbContext and models
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.InMemory; // Add this to ensure the extension method is available

namespace CoreWebAPI.Tests
{
    // Ensure the required package is installed: Microsoft.EntityFrameworkCore.InMemory
    // You can install it via NuGet Package Manager or the Package Manager Console:
    // Install-Package Microsoft.EntityFrameworkCore.InMemory
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<InventoryDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add an in-memory database for testing
                services.AddDbContext<InventoryDbContext>(options =>
                    options.UseInMemoryDatabase("InMemoryInventoryDb")); // Ensure the required package is referenced

                // Ensure the database is created
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                    db.Database.EnsureCreated();

                    // Optionally seed the database with test data
                    SeedTestData(db);
                }
            });
        }

        private static void SeedTestData(InventoryDbContext db)
        {
            // Seed the in-memory database with test data  
            db.Products.AddRange(
                new Product
                {
                    ProductId = 1,
                    ProductName = "Product1",
                    CategoryId = 1,
                    Price = 10.99m,
                    Stock = 100,
                    Category = new Category
                    {
                        CategoryId = 1,
                        CategoryName = "Category1",
                        SupplierId = 1,
                        Supplier = new Supplier
                        {
                            SupplierId = 1,
                            SupplierName = "Supplier1",
                            ContactName = "Contact1",
                            Phone = "123-456-7890"
                        }
                    }
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Product2",
                    CategoryId = 2,
                    Price = 15.49m,
                    Stock = 50,
                    Category = new Category
                    {
                        CategoryId = 2,
                        CategoryName = "Category2",
                        SupplierId = 2,
                        Supplier = new Supplier
                        {
                            SupplierId = 2,
                            SupplierName = "Supplier2",
                            ContactName = "Contact2",
                            Phone = "987-654-3210"
                        }
                    }
                }
            );

            db.Categories.AddRange(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Category1",
                    SupplierId = 1,
                    Supplier = new Supplier
                    {
                        SupplierId = 1,
                        SupplierName = "Supplier1",
                        ContactName = "Contact1",
                        Phone = "123-456-7890"
                    }
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Category2",
                    SupplierId = 2,
                    Supplier = new Supplier
                    {
                        SupplierId = 2,
                        SupplierName = "Supplier2",
                        ContactName = "Contact2",
                        Phone = "987-654-3210"
                    }
                }
            );

            db.Suppliers.AddRange(
                new Supplier { SupplierId = 1, SupplierName = "Supplier1", ContactName = "Contact1", Phone = "123-456-7890" },
                new Supplier { SupplierId = 2, SupplierName = "Supplier2", ContactName = "Contact2", Phone = "987-654-3210" }
            );

            db.SaveChanges();
        }
    }
}
