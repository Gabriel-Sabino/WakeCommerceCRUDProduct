using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Application.Services;
using WakeCommerceCRUDProduct.Domain.Entities;

namespace WakeCommerceCRUDProduct.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var products = await _productService.GetAllProductAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {

            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product != null)
                return Ok(product);

                return NotFound("Produto Nao Encontrado");
            }
            catch
            {
                return NotFound("Produto Nao Encontrado");
            }
            
        }

        [HttpGet("GetByName/{name}")]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            try
            {
                var product = await _productService.GetProductByNameAsync(name);

                if (product != null)
                    return Ok(product);

                return NotFound("Produto Nao Encontrado");
            }
            catch
            {
                return NotFound("Produto Nao Encontrado");
            }
        }

        [HttpGet("GetOrderByNameOrStockOrValue/{name}")]
        public async Task<IActionResult> OrderByProductAsync(string name)
        {
            try 
            {
                var product = await _productService.OrderByProductListAsync(name);
                if (product == null || !product.Any())
                    return BadRequest();

                return Ok(product);
            }
            catch 
            {
                return BadRequest();
            }
            
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = new Product(productDTO.Name, productDTO.Stock, productDTO.Value);

                var createdProduct = await _productService.CreateProductAsync(product);
                return Ok(createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ProductDTO productDTO)
        {
            var product = new Product(productDTO.Name, productDTO.Stock, productDTO.Value);


            int existingProduct = await _productService.UpdateProductAsync(id, product);
            if(existingProduct == 0)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int product = await _productService.DeleteProductAsync(id);
            if(product == 0)
                return BadRequest();

            return NoContent();
        }


    }
}
