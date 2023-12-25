using CommonLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using Services;
using CommonLibrary.Repositories;
using CommonLibrary.Caching;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Services.BackGroundServices;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MaterialDBContext>(opt =>
{
    opt.UseSqlServer("Name=MaterialDBConnection", b => b.MigrationsAssembly("API_Material"));
});

builder.Services.AddAutoMapper(typeof(CommonLibrary.Mapping.MappingConfig));

builder.Services.AddScoped<ICacheManager, CacheManager>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddStackExchangeRedisCache(opt => { opt.Configuration = configuration.GetSection("Redis").Value; });

CorsPolicyBuilder cbuilder = new CorsPolicyBuilder().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
CorsPolicy policy = cbuilder.Build();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("MyCors", policy);
});

builder.Services.AddHostedService<MaterialBackgroundService>();


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
app.UseCors("MyCors");


app.Run();
