using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerceCRUDProduct.Domain.Entities;

namespace WakeCommerceCRUDProduct.Domain.Interfaces.Cache
{
    public interface ICacheInMemory
    {
        Task<IEnumerable<Product>> GetProductsFromCacheOrRepositoryAsync(string cacheKey);
        Task UpdateCache();
    }
}
