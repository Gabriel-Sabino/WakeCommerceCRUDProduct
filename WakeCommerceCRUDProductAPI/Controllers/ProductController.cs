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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var product = await _productService.GetAllProductAsync();
            return Ok(product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if(product == null)
                return NotFound();
            
            return Ok(product);
        }

        [HttpGet("product/{name}")]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            var product = await _productService.GetProductByNameAsync(name);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("orderby/{name}")]
        public async Task<IActionResult> OrderByProductAsync(string name)
        {
            try 
            {
                var product = await _productService.OrderByProductListAsync(name);


                return Ok(product);

            }
            catch (ArgumentNullException ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost]
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
                return CreatedAtAction(nameof(GetByIdAsync), new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ProductDTO productDTO)
        {
            var product = new Product(productDTO.Name, productDTO.Stock, productDTO.Value);


            int existingProduct = await _productService.UpdateProductAsync(id, product);
            if(existingProduct == 0)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int product = await _productService.DeleteProductAsync(id);
            if(product == 0)
                return BadRequest();

            return NoContent();
        }
    }
}
