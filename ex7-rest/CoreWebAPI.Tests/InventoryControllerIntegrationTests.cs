namespace CoreWebAPI.Tests
{
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CoreWebApi;
    using DataLibrary;

    [TestClass]
    public class InventoryControllerIntegrationTests
    {
        private static WebApplicationFactory<CoreWebApi.Program>? _factory;
        private static HttpClient? _client;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Initialize the WebApplicationFactory and HttpClient
            _factory = new WebApplicationFactory<CoreWebApi.Program>();
            _client = _factory.CreateClient();
        }

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void ClassCleanup()
        {
            if (_client != null)
            {
                _client.Dispose();
            }

            if (_factory != null)
            {
                _factory.Dispose();
            }
        }

        [TestMethod]
        public async Task GetProducts_ReturnsOk_WithProducts()
        {
            // Act
            var response = await _client!.GetAsync("/api/Inventory/Products");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<Product[]>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Length > 0); // Assuming the database is seeded with data
        }

        [TestMethod]
        public async Task GetProductById_ExistingId_ReturnsOk_WithProduct()
        {
            // Act
            var response = await _client!.GetAsync("/api/Inventory/Products/1");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.ProductId);
        }

        [TestMethod]
        public async Task GetProductById_NonExistingId_ReturnsNotFound()
        {
            // Act
            var response = await _client!.GetAsync("/api/Inventory/Products/999");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseBody.Contains("Product with ID 999 not found."));
        }
    }
}
