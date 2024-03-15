using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerceCRUDProduct.Domain.Entities;

namespace WakeCommerceCRUDProduct.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
       
        public ProductDbContext(DbContextOptions<ProductDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().HasKey(p => p.Id);
            builder.Entity<Product>().ToTable("Products");
            builder.Entity<Product>().Property(p => p.Value).HasColumnType("decimal(18, 2)");
            builder.Entity<Product>().Property(p => p.Name).IsRequired();
        }

    }
}
