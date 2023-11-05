using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.CQRS.Commands;
using Services.CQRS.Handlers;
using Services.CQRS.Notifications;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PurchaseDBContext>(opt =>
{
    opt.UseSqlServer("Name=PurchaseDBConnection", b => b.MigrationsAssembly("API_Purchase"));
});

builder.Services.AddAutoMapper(typeof(CommonLibrary.Mapping.MappingConfig));

builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ICacheManager, CacheManager>();
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddStackExchangeRedisCache(opt => { opt.Configuration = "localhost:6379"; });

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.Lifetime = ServiceLifetime.Scoped;
});

builder.Services.AddScoped<IRequestHandler<AddPurchaseCommand, Purchase>, AddPurchaseCommandHandler>();
builder.Services.AddScoped<INotificationHandler<PurchaseAddedNotification>, PurchaseAddedNotificationHandler>();

builder.Services.AddScoped<IRequestHandler<AddPurchaseDetailCommand, PurchaseDetail>, AddPurchaseDetailCommandHandler>();
builder.Services.AddScoped<INotificationHandler<PurchaseDetailAddedNotification>, PurchaseDetailAddedNotificationHandler>();

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
