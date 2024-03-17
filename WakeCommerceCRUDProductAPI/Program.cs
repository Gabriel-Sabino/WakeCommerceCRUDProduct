using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Application.Services;
using WakeCommerceCRUDProduct.Domain.Entities;
using WakeCommerceCRUDProduct.Domain.Interfaces.Repositories;
using WakeCommerceCRUDProduct.Infrastructure.Data;
using WakeCommerceCRUDProduct.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//documentacao API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WakeCommerceCRUDProduct",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Gabriel Sabino",
            Email = "gabrielsabino1505@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/gabriel-sabino1/")
        }
    });

    var xmlFile = "WakeCommerceCRUDProduct.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

var app = builder.Build();

//Inserindo informacoes no banco ao iniciar o projeto
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

    dbContext.Database.EnsureCreated();

    if (!dbContext.Products.Any())
    {
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
