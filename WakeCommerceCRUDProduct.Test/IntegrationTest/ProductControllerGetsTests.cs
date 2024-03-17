using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WakeCommerceCRUDProduct.API.Controllers;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Infrastructure.Data;
using WakeCommerceCRUDProduct.Application.Services;
using Xunit;
using WakeCommerceCRUDProduct.Infrastructure.Repositories;
using Moq;

namespace WakeCommerceCRUDProduct.Tests
{
    public class ProductControllerIntegrationTests : IDisposable
    {
        private readonly DbContextOptions<ProductDbContext> _options;

        public ProductControllerIntegrationTests()
        {
            _options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        public void Dispose()
        {
            using (var context = new ProductDbContext(_options))
            {
                context.Database.EnsureDeleted();
            }
        }

        private ProductDbContext GetDbContext()
        {
            return new ProductDbContext(_options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                context.Products.AddRange(
                    new Product("Product 1", 10, 100),
                    new Product("Product 2", 20, 200),
                    new Product("Product 3", 30, 300)
                );
                context.SaveChanges();
            }

            using (var context = GetDbContext()) 
            {
                var repository = new ProductRepository(context);
                var productService = new ProductService(repository);
                var controller = new ProductController(productService);

                // Act
                var result = await controller.GetAllAsync();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);
                Assert.Equal(3, products.Count());
            }
        }

        [Fact]
        public async Task GetAllAsync_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var productServiceMock = new Mock<IProductService>();
                productServiceMock.Setup(x => x.GetAllProductAsync()).ThrowsAsync(new Exception("Something went wrong"));
                var controller = new ProductController(productServiceMock.Object);

                // Act
                var result = await controller.GetAllAsync();

                // Assert
                var statusCodeResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal(500, statusCodeResult.StatusCode);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var productName = "Product 1";
            var productStock = 10;
            var productValue = 100;

            using (var context = GetDbContext())
            {
                var product = new Product(productName, productStock, productValue);

                product.ReceiveId(productId);

                context.Products.Add(product);
                context.SaveChanges();
            }

            using (var context = GetDbContext())
            {
                var repository = new ProductRepository(context);
                var productService = new ProductService(repository);
                var controller = new ProductController(productService);

                // Act
                var result = await controller.GetByIdAsync(productId);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var product = Assert.IsType<ProductDTO>(okResult.Value);
                Assert.Equal(productName, product.Name);
                Assert.Equal(productStock, product.Stock);
                Assert.Equal(productValue, product.Value);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;

            using (var context = GetDbContext())
            {
                // Product vazio
            }

            using (var context = GetDbContext())
            {
                var repository = new ProductRepository(context);
                var productService = new ProductService(repository);
                var controller = new ProductController(productService);

                // Act
                var result = await controller.GetByIdAsync(productId);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal("Produto Nao Encontrado", notFoundResult.Value);
            }
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var productName = "Product 1";
            var product = new ProductDTO { Name = productName };
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.GetProductByNameAsync(productName)).ReturnsAsync(product);

            using (var context = GetDbContext())
            {
                context.Products.Add(new Product(productName, 10, 100));
                context.SaveChanges();
            }

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetByNameAsync(productName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal(productName, returnedProduct.Name);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productName = "Nonexistent Product";
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.GetProductByNameAsync(productName)).ReturnsAsync((ProductDTO)null!);

            using (var context = GetDbContext())
            {
                var controller = new ProductController(productServiceMock.Object);

                // Act
                var result = await controller.GetByNameAsync(productName);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }
        }
    }
}
