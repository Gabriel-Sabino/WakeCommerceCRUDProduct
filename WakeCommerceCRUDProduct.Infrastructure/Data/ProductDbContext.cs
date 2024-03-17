using Microsoft.EntityFrameworkCore;
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
