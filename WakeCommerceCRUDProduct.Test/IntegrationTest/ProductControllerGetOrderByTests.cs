using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerceCRUDProduct.API.Controllers;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Application.Services;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Infrastructure.Cache;
using WakeCommerceCRUDProduct.Infrastructure.Data;
using WakeCommerceCRUDProduct.Infrastructure.Repositories;

namespace WakeCommerceCRUDProduct.Test.IntegrationTest
{
    public class ProductControllerGetOrderByTests : IDisposable
    {
        private readonly DbContextOptions<ProductDbContext> _options;

        public ProductControllerGetOrderByTests()
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
        public async Task OrderByProductNameAsync_ReturnsOrderedProducts()
        {

            using (var context = GetDbContext())
            {
                context.Products.AddRange(
                    new Product("Product 2", 20, 200),
                    new Product("Product 1", 10, 100),
                    new Product("Product 3", 30, 300)
                );
                context.SaveChanges();
            }

            using (var context = GetDbContext())
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
                var productService = new ProductService(cacheInMemory, new ProductRepository(context));
                var controller = new ProductController(productService);

                // Act
                var result = await controller.OrderByProductAsync("Name");

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);

                var orderedProducts = products.OrderBy(p => p.Name).ToList();

                Assert.Equal("Product 1", orderedProducts[0].Name);
                Assert.Equal("Product 2", orderedProducts[1].Name);
                Assert.Equal("Product 3", orderedProducts[2].Name);
            }
        }

        [Fact]
        public async Task OrderByProductStockAsync_ReturnsOrderedProducts()
        {

            using (var context = GetDbContext())
            {
                context.Products.AddRange(
                    new Product("Product 1", 20, 200),
                    new Product("Product 2", 10, 100),
                    new Product("Product 3", 30, 300)
                );
                context.SaveChanges();
            }

            using (var context = GetDbContext())
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
                var productService = new ProductService(cacheInMemory, new ProductRepository(context));
                var controller = new ProductController(productService);

                // Act
                var result = await controller.OrderByProductAsync("Stock");

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);

                var orderedProducts = products.OrderBy(p => p.Stock).ToList();

                Assert.Equal("Product 2", orderedProducts[0].Name);
                Assert.Equal("Product 1", orderedProducts[1].Name);
                Assert.Equal("Product 3", orderedProducts[2].Name);
            }
        }

        [Fact]
        public async Task OrderByProductValueAsync_ReturnsOrderedProducts()
        {

            using (var context = GetDbContext())
            {
                context.Products.AddRange(
                    new Product("Product 2", 20, 200),
                    new Product("Product 3", 10, 100),
                    new Product("Product 1", 30, 300)
                );
                context.SaveChanges();
            }

            using (var context = GetDbContext())
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
                var productService = new ProductService(cacheInMemory, new ProductRepository(context));
                var controller = new ProductController(productService);

                // Act
                var result = await controller.OrderByProductAsync("Value");

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);

                var orderedProducts = products.OrderBy(p => p.Value).ToList();

                Assert.Equal("Product 3", orderedProducts[0].Name);
                Assert.Equal("Product 2", orderedProducts[1].Name);
                Assert.Equal("Product 1", orderedProducts[2].Name);
            }
        }

        [Fact]
        public async Task OrderByProductAsync_ReturnsBadRequestOnException()
        {
            

            using (var context = GetDbContext())
            {
                context.Products.AddRange(
                    new Product("Product 2", 20, 200),
                    new Product("Product 1", 10, 100),
                    new Product("Product 3", 30, 300)
                );
                context.SaveChanges();
            }

            using (var context = GetDbContext())
            {
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var cacheInMemory = new CacheInMemory(memoryCache, new ProductRepository(context));
                var productService = new ProductService(cacheInMemory, new ProductRepository(context));
                var controller = new ProductController(productService);

                // Act
                var result = await controller.OrderByProductAsync("invalid");

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            }
        }

    }
}
