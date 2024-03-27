using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using WakeCommerceCRUDProduct.API.Controllers;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Domain.Entities;

namespace WakeCommerceCRUDProduct.Test.UnitTest
{
    public class ProductControllerGetsTests
    {
        [Fact]
        public async Task GetByIdAsync_ProductExists_ReturnsOk()
        {
            // Arrange
            int productId = 1;
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(new ProductDTO { Name = "Test Product", Stock = 10, Value = 100 });

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetByIdAsync(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(10, product.Stock);
            Assert.Equal(100, product.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            int productId = 1;
            var productServiceMock = new Mock<IProductService>();
            _ = productServiceMock.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(null as ProductDTO);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetByIdAsync(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Produto Nao Encontrado", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsProducts()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            var expectedProducts = new List<ProductDTO>
            {
                new() { Name = "Product 1", Stock = 10, Value = 100 },
                new() { Name = "Product 2", Stock = 15, Value = 150 }
            };
            productServiceMock.Setup(x => x.GetAllProductAsync()).ReturnsAsync(expectedProducts);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);
            Assert.Equal(expectedProducts.Count, products.Count());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.GetAllProductAsync()).ReturnsAsync(new List<ProductDTO>());

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);
            Assert.Empty(products);
        }

        [Fact]
        public async Task GetAllAsync_HandlesException()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.GetAllProductAsync()).ThrowsAsync(new Exception("Simulated Exception"));

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetAllAsync();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetByNameAsync_ProductExists_ReturnsOk()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            var expectedProduct = new ProductDTO { Name = "Test Product", Stock = 10, Value = 100 };
            productServiceMock.Setup(x => x.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(expectedProduct);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetByNameAsync("Test Product");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal(expectedProduct, product);
        }

        [Fact]
        public async Task GetByNameAsync_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync(null as ProductDTO);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetByNameAsync("Nonexistent Product");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task OrderByProductAsync_ProductsOrdered_ReturnsOk()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            var expectedProducts = new List<ProductDTO>
            {
                new() { Name = "Product A", Stock = 10, Value = 100 },
                new() { Name = "Product B", Stock = 5, Value = 50 },
                new() { Name = "Product C", Stock = 20, Value = 200 }
            };
            productServiceMock.Setup(x => x.OrderByProductListAsync(It.IsAny<string>())).ReturnsAsync(expectedProducts);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.OrderByProductAsync("Name");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);
            Assert.Equal(expectedProducts, products);
        }

        [Fact]
        public async Task OrderByProductAsync_InvalidArgument_ReturnsBadRequest()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.OrderByProductListAsync(It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("name", "Simulated exception"));

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.OrderByProductAsync("InvalidField");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

    }
}
