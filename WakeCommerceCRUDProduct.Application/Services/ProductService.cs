using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<Product> CreateProductAsync(Product product)
        {
            if(product.Value < 0)
            {
                throw new ArgumentException("O valor do produto não pode ser negativo.");
            }
            return await _productRepository.CreateProductAsync(product);
        }

        public async Task<int> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _productRepository.GetAllProductAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<int> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null) 
            {
                throw new InvalidOperationException("Produto não encontrado.");
            }

            existingProduct.UpdateName(product.Name);
            existingProduct.UpdateStock(product.Stock);
            existingProduct.UpdateValue(product.Value);

            return await _productRepository.UpdateProductAsync(id, existingProduct);
        }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
             throw new ArgumentNullException("Necessario popular o campo");
            

            var product = await _productRepository.GetProductByNameAsync(name);

            if (product == null)
                throw new InvalidOperationException("Produto nao encontrado");

            return product;
        }

        public async Task<List<Product>> OrderByProductListAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Necessario escolher o campo que deseja ordenar");

            var productList = new List<Product>();
            name.ToLower();

            if (name == "name")
                productList = await _productRepository.OrderByNameProductListAsync(name);
            else if (name == "stock")
                productList = await _productRepository.OrderByStockProductListAsync(name);
            else if (name == "value")
                productList = await _productRepository.OrderByValueProductListAsync(name);

            return productList;
        }
    }
}
