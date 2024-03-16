using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using WakeCommerceCRUDProduct.API.Controllers;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Domain.Entities;

namespace WakeCommerceCRUDProduct.Test.UnitTest
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task CreateAsync_ValidProduct_ReturnsOk()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(new ProductDTO { Name = "Test Product", Stock = 10, Value = 100 });

            var controller = new ProductController(productServiceMock.Object);
            var productDTO = new ProductDTO { Name = "Test Product", Stock = 10, Value = 100 };

            // Act
            var result = await controller.CreateAsync(productDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdProduct = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal("Test Product", createdProduct.Name);
            Assert.Equal(10, createdProduct.Stock);
            Assert.Equal(100, createdProduct.Value);
        }

        [Fact]
        public async Task CreateAsync_InvalidProduct_ReturnsBadRequest()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            var controller = new ProductController(productServiceMock.Object);
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await controller.CreateAsync(new ProductDTO());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ExistingProduct_ReturnsNoContent()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.UpdateProductAsync(It.IsAny<int>(), It.IsAny<Product>())).ReturnsAsync(1);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.UpdateAsync(1, new ProductDTO { Name = "Updated Product", Stock = 20, Value = 200 });

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingProduct_ReturnsBadRequest()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.UpdateProductAsync(It.IsAny<int>(), It.IsAny<Product>())).ReturnsAsync(0);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.UpdateAsync(999, new ProductDTO { Name = "Updated Product", Stock = 20, Value = 200 });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingProduct_ReturnsNoContent()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.DeleteProductAsync(It.IsAny<int>())).ReturnsAsync(1);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingProduct_ReturnsBadRequest()
        {
            // Arrange
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.DeleteProductAsync(It.IsAny<int>())).ReturnsAsync(0);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.Delete(999);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
