using Microsoft.EntityFrameworkCore;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Repositories;
using WakeCommerceCRUDProduct.Infrastructure.Data;

namespace WakeCommerceCRUDProduct.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductRepository(ProductDbContext productDbContext)
        {
            _dbContext = productDbContext;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<int> DeleteProductAsync(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                return await _dbContext.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

        }

        public async Task<int> UpdateProductAsync(int id, Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.Name == name);

            return product;
        }

        public async Task<List<Product>> OrderByNameProductListAsync()
        {
            return await _dbContext.Products.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<List<Product>> OrderByStockProductListAsync()
        {
            return await _dbContext.Products.OrderBy(x => x.Stock).ToListAsync();
        }

        public async Task<List<Product>> OrderByValueProductListAsync()
        {
            return await _dbContext.Products.OrderBy(x => x.Value).ToListAsync();
        }
    }
}
