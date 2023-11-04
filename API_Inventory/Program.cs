using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.BackGroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDBContext>(opt =>
{
    opt.UseSqlServer("Name=InventoryDBConnection", b => b.MigrationsAssembly("API_Inventory"));
});

builder.Services.AddAutoMapper(typeof(CommonLibrary.Mapping.MappingConfig));

builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ICacheManager, CacheManager>();
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddStackExchangeRedisCache(opt => { opt.Configuration = "localhost:6379"; });

builder.Services.AddHostedService<InventoryBackgroundService>();


var app = builder.Build();

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
