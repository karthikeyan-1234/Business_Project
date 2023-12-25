using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

using Services;
using Services.CQRS.Commands.Purchase_Commands;
using Services.CQRS.Handlers.Inventory.Broker;
using Services.CQRS.Handlers.Inventory_Handlers;
using Services.CQRS.Handlers.Purchase_Handlers;
using Services.CQRS.Notifications.Inventory_Notifications;
using Services.CQRS.Queries.Inventory;
using Services.CQRS.Queries.Purchases;

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

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.Lifetime = ServiceLifetime.Scoped;
});

builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ICacheManager, CacheManager>();
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddStackExchangeRedisCache(opt => { opt.Configuration = "localhost:6379"; });

CorsPolicyBuilder cbuilder = new CorsPolicyBuilder().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
CorsPolicy policy = cbuilder.Build();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("MyCors", policy);
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.Lifetime = ServiceLifetime.Scoped;
});

builder.Services.AddScoped<IRequestHandler<AddPurchaseCommand, Purchase>, AddPurchaseCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AddPurchaseDetailCommand, PurchaseDetail>, AddPurchaseDetailCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdatePurchaseDetailCommand, PurchaseDetail>, UpdatePurchaseDetailCommandHandler>();
builder.Services.AddScoped<INotificationHandler<UpdateInventoryNotification>, UpdateInventoryNotificationHandler>();
builder.Services.AddScoped<IRequestHandler<GetItemInventoryQueryBroker, Inventory>, GetItemInventoryQueryBrokerHandler>();
builder.Services.AddScoped<IRequestHandler<DeletePurchaseDetailCommand, PurchaseDetail>, DeletePurchaseDetailCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetPurchaseDetailsByDateQuery, IEnumerable<PurchaseDetail>>, GetPurchaseDetailsByDateQueryHandler>();

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

app.UseCors("MyCors");

app.Run();
