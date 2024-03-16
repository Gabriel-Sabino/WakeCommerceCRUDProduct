using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerceCRUDProduct.Domain.Entities;

namespace WakeCommerceCRUDProduct.Domain.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> GetProductByNameAsync(string name);
        Task<List<Product>> OrderByNameProductListAsync();
        Task<List<Product>> OrderByStockProductListAsync();
        Task<List<Product>> OrderByValueProductListAsync();
        Task<Product> CreateProductAsync(Product product);
        Task<int> UpdateProductAsync(int id, Product product);
        Task<int> DeleteProductAsync(int id);
    }
}
