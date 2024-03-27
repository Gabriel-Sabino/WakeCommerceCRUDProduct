using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Cache;
using WakeCommerceCRUDProduct.Domain.Interfaces.Repositories;

namespace WakeCommerceCRUDProduct.Infrastructure.Cache
{
    public class CacheInMemory(IMemoryCache memoryCache, IProductRepository productRepository) : ICacheInMemory
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IProductRepository _productRepository = productRepository;

        private static MemoryCacheEntryOptions SetTimeSpanCache()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(8),
                SlidingExpiration = TimeSpan.FromSeconds(5)
            };
            return cacheEntryOptions;
        }

        public async Task<IEnumerable<Product>> GetProductsFromCacheOrRepositoryAsync(string cacheKey)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product> productCache))
            {
                productCache = await _productRepository.GetAllProductAsync();

                var cacheEntryOptions = SetTimeSpanCache();
                _memoryCache.Set(cacheKey, productCache, cacheEntryOptions);

                return productCache;
            }

            return productCache;
        }


        public async Task UpdateCache()
        {
            var updatedProducts = await _productRepository.GetAllProductAsync()
                ?? throw new InvalidOperationException("Produtos não encontrados");

            var cacheEntryOptions = SetTimeSpanCache();

            _memoryCache.Set("getallproducts", updatedProducts, cacheEntryOptions);
        }

    }
}
