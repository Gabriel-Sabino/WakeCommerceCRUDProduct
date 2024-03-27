using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WakeCommerceCRUDProduct.API.Controllers;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Services;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Cache;
using WakeCommerceCRUDProduct.Infrastructure.Cache;
using WakeCommerceCRUDProduct.Infrastructure.Data;
using WakeCommerceCRUDProduct.Infrastructure.Repositories;

namespace WakeCommerceCRUDProduct.Test.IntegrationTest
{
    public class ProductControllerEditTests : IDisposable
    {
        private readonly DbContextOptions<ProductDbContext> _options;

        public ProductControllerEditTests()
        {
            _options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        public void Dispose()
        {
            using var context = new ProductDbContext(_options);
            context.Database.EnsureDeleted();

            GC.SuppressFinalize(this);
        }

        private ProductDbContext GetDbContext()
        {
            return new ProductDbContext(_options);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult_WhenProductIsValid()
        {
            // Arrange
            using var context = GetDbContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
            var productService = new ProductService(cacheInMemory, new ProductRepository(context));
            var controller = new ProductController(productService);
            var productDTO = new ProductDTOCreateAndUpdate { Name = "Test Product", Stock = 10, Value = "100" };

            // Act
            var result = await controller.CreateAsync(productDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdProduct = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal(productDTO.Name, createdProduct.Name);
            Assert.Equal(productDTO.Stock, createdProduct.Stock);
            Assert.Equal(productDTO.Value, createdProduct.Value.ToString());
        }


        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_WhenModelStateIsNotValid()
        {
            // Arrange
            using var context = GetDbContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
            var productService = new ProductService(cacheInMemory, new ProductRepository(context));
            var controller = new ProductController(productService);
            controller.ModelState.AddModelError("Name", "Name is required");
            var productDTO = new ProductDTOCreateAndUpdate { Name = "", Stock = 0, Value = "" };

            // Act
            var result = await controller.CreateAsync(productDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            using var context = GetDbContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
            var productService = new ProductService(cacheInMemory, new ProductRepository(context));
            var controller = new ProductController(productService);
            var productDTO = new ProductDTOCreateAndUpdate { Name = "Test Product", Stock = 10, Value = "100" };

            productDTO.Name = null;

            // Act
            var result = await controller.CreateAsync(productDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<string>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenProductIsUpdatedSuccessfully()
        {
            // Arrange
            var productId = 1;
            var productName = "Updated Product";
            var productStock = 20;
            var productValue = "200";

            using (var context = GetDbContext())
            {
                var existingProduct = new Product("Existing Product", 10, 100);
                existingProduct.ReceiveId(productId);
                context.Products.Add(existingProduct);
                context.SaveChanges();
            }

            using (var context = GetDbContext())
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
                var productService = new ProductService(cacheInMemory, new ProductRepository(context));
                var controller = new ProductController(productService);

                var updatedProductDTO = new ProductDTOCreateAndUpdate { Name = productName, Stock = productStock, Value = productValue };

                // Act
                var result = await controller.UpdateAsync(productId, updatedProductDTO);

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenProductIsDeletedSuccessfully()
        {
            // Arrange
            var productId = 1;

            using (var context = GetDbContext())
            {
                var existingProduct = new Product("Existing Product", 10, 100);
                existingProduct.ReceiveId(productId);
                context.Products.Add(existingProduct);
                context.SaveChanges();
            }

            using (var context = GetDbContext())
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
                var productService = new ProductService(cacheInMemory, new ProductRepository(context));
                var controller = new ProductController(productService);

                // Act
                var result = await controller.Delete(productId);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenProductIdDoesNotExist()
        {
            // Arrange
            var nonExistentProductId = 999;

            using (var context = GetDbContext())
            {
                // Product inexistente
            }

            using (var context = GetDbContext())
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
                var productService = new ProductService(cacheInMemory, new ProductRepository(context));
                var controller = new ProductController(productService);

                // Act
                var result = await controller.Delete(nonExistentProductId);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

    }
}
