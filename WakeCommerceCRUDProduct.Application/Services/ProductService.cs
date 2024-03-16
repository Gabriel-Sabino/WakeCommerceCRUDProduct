using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Repositories;

namespace WakeCommerceCRUDProduct.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDTO> CreateProductAsync(Product productInfo)
        {
            if(productInfo.Value < 0 || productInfo.Name.IsNullOrEmpty() || productInfo.Stock < 0)
            {
                throw new ArgumentException("O produto deve estar preenchido.");
            }


            var product = await _productRepository.CreateProductAsync(productInfo);

            ProductDTO productDTO = new()
            {
                Name = product.Name,
                Stock = product.Stock,
                Value = product.Value
            };

            return productDTO;


        }

        public async Task<int> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductAsync()
        {
            var product = await _productRepository.GetAllProductAsync();

            IEnumerable<ProductDTO> productDTOs = product
                .Select(product => new ProductDTO
        {
            Name = product.Name,
            Stock = product.Stock,
            Value = product.Value
        }).ToList();

            return productDTOs;

        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

                ProductDTO productDTO = new()
                {
                    Name = product.Name,
                    Stock = product.Stock,
                    Value = product.Value
                };
                
                return productDTO;
        }

        public async Task<int> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id) 
                ?? throw new InvalidOperationException("Produto não encontrado para atualizar.");

            existingProduct.UpdateName(product.Name);
            existingProduct.UpdateStock(product.Stock);
            existingProduct.UpdateValue(product.Value);
            existingProduct.UpdateModifiedAt();

            return await _productRepository.UpdateProductAsync(id, existingProduct);
        }

        public async Task<ProductDTO> GetProductByNameAsync(string name)
        {            

            var product = await _productRepository.GetProductByNameAsync(name) 
                ?? throw new InvalidOperationException("Produto nao encontrado");

            ProductDTO productDTO = new()
            {
                Name = product.Name,
                Stock = product.Stock,
                Value = product.Value
            };

            return productDTO;
        }

        public async Task<IEnumerable<ProductDTO>> OrderByProductListAsync(string name)
        {
            var productList = new List<Product>();
            var orderByNameStockValue = name.ToLower();

            if (orderByNameStockValue == "name")
                productList = await _productRepository.OrderByNameProductListAsync();
            else if (orderByNameStockValue == "stock")
                productList = await _productRepository.OrderByStockProductListAsync();
            else if (orderByNameStockValue == "value")
                productList = await _productRepository.OrderByValueProductListAsync();


            if (productList == null || !productList.Any())
            {
                throw new InvalidOperationException("Produto nao encontrado");
            }

            IEnumerable<ProductDTO> productDTOs = productList
                .Select(product => new ProductDTO
                {
                    Name = product.Name,
                    Stock = product.Stock,
                    Value = product.Value
                }).ToList();

            return productDTOs;
        }
    }
}
