using CommonLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using Services;
using CommonLibrary.Repositories;
using CommonLibrary.Caching;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PurchaseDBContext>(opt =>
{
    opt.UseSqlServer("Name=PurchaseDBConnection", b => b.MigrationsAssembly("API_Material"));
});

builder.Services.AddAutoMapper(typeof(CommonLibrary.Mapping.MappingConfig));

builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ICacheManager, CacheManager>();
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
