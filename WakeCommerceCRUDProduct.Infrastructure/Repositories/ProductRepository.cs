﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
           return await _dbContext.Products
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();
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
            //duas opcoes para realizar o update

            _dbContext.Entry(product).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync();

            //return await _dbContext.Products.Where(x => x.Id == id)
            //    .ExecuteUpdateAsync(setters => setters
            //    .SetProperty(o => o.Id, product.Id)
            //    .SetProperty(o => o.Name, product.Name)
            //    .SetProperty(o => o.Stock, product.Stock)
            //    .SetProperty(o => o.Value, product.Value));
        }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.Name == name);

            return product;
        }

        public async Task<List<Product>> OrderByNameProductListAsync(string name)
        {
            return await _dbContext.Products.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<List<Product>> OrderByStockProductListAsync(string name)
        {
            return await _dbContext.Products.OrderBy(x => x.Stock).ToListAsync();
        }

        public async Task<List<Product>> OrderByValueProductListAsync(string name)
        {
            return await _dbContext.Products.OrderBy(x => x.Value).ToListAsync();
        }
    }
}
