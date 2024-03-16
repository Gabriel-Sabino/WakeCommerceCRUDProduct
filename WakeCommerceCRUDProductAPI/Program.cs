using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Application.Services;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Repositories;
using WakeCommerceCRUDProduct.Infrastructure.Data;
using WakeCommerceCRUDProduct.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

    // Certifique-se de que o banco de dados foi criado
    dbContext.Database.EnsureCreated();

    // Verifique se existem alguns produtos no banco de dados
    if (!dbContext.Products.Any())
    {
        // Adicione algumas linhas ao banco de dados
        dbContext.Products.AddRange(
                    new Product("Product 1", 10, 100),
                    new Product("Product 2", 20, 200),
                    new Product("Product 3", 30, 300),
                    new Product("Product 4", 40, 400),
                    new Product("Product 5", 50, 500)
        );

        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
