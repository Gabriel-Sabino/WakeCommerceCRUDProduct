using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Cache;
using WakeCommerceCRUDProduct.Domain.Interfaces.Repositories;

namespace WakeCommerceCRUDProduct.Application.Services
{
    public class ProductService(ICacheInMemory cacheInMemory, IProductRepository productRepository) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICacheInMemory _cacheInMemory = cacheInMemory;

        public async Task<ProductDTO> CreateProductAsync(Product productInfo)
        {
            if(productInfo.Value < 0 || productInfo.Name.IsNullOrEmpty() || productInfo.Stock < 0)
            {
                throw new ArgumentException("O produto deve estar preenchido com informações validas.");
            }

            var product = await _productRepository.CreateProductAsync(productInfo);

            await _cacheInMemory.UpdateCache();

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
            var deletedProduct =  await _productRepository.DeleteProductAsync(id);

            if(deletedProduct == 0)
            {
                throw new ArgumentException("Produto Não encontrado para deletar");
            }

            await _cacheInMemory.UpdateCache();

            return deletedProduct;
        }

        public async Task<int> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id)
                ?? throw new InvalidOperationException("Produto não encontrado para atualizar.");

            existingProduct.UpdateName(product.Name);
            existingProduct.UpdateStock(product.Stock);
            existingProduct.UpdateValue(product.Value);
            existingProduct.UpdateModifiedAt();

            var updatedProduct = await _productRepository.UpdateProductAsync(id, existingProduct);
            await _cacheInMemory.UpdateCache();

            return updatedProduct;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductAsync()
        {
            var productCache = await _cacheInMemory.GetProductsFromCacheOrRepositoryAsync("getallproducts");

            var productDTOs = productCache.Select(product => new ProductDTO
            {
                Name = product.Name,
                Stock = product.Stock,
                Value = product.Value
            }).ToList();

            return productDTOs;
        }



        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id)
            ?? throw new InvalidOperationException("Produto não encontrado.");

            ProductDTO productDTO = new()
                {
                    Name = product.Name,
                    Stock = product.Stock,
                    Value = product.Value
                };
                
                return productDTO;
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
            var orderName = name.ToLower();

            var orderedProduct = await _cacheInMemory.GetProductsFromCacheOrRepositoryAsync("getallproducts");

            if (orderName == "name")
                orderedProduct = orderedProduct.OrderBy(x => x.Name);
            else if (orderName == "stock")
                orderedProduct = orderedProduct.OrderBy(x => x.Stock);
            else if (orderName == "value")
                orderedProduct = orderedProduct.OrderBy(x => x.Value);
            else
            {
                throw new ArgumentException("Digite entre as opcoes: Name, Stock ou Value");
            }
            var productDTOs = orderedProduct.Select(product => new ProductDTO
            {
                Name = product.Name,
                Stock = product.Stock,
                Value = product.Value
            }).ToList();

            return productDTOs;
        }


    }
}
