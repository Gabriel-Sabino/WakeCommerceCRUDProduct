using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Repositories;

namespace WakeCommerceCRUDProduct.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<ProductDTO> GetProductByNameAsync(string name);
        Task<IEnumerable<ProductDTO>> OrderByProductListAsync(string name);
        Task<ProductDTO> CreateProductAsync(Product product);
        Task<int> UpdateProductAsync(int id, Product product);
        Task<int> DeleteProductAsync(int id);

    }
}
