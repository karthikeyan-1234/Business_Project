using Microsoft.AspNetCore.Cors.Infrastructure;

using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(opt =>
{
    opt.EnableDetailedErrors = true;
    opt.KeepAliveInterval = TimeSpan.FromHours(2);
});

CorsPolicyBuilder cbuilder = new CorsPolicyBuilder().AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:8080").AllowCredentials();
CorsPolicy policy = cbuilder.Build();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("MyCors", policy);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.UseCors("MyCors");

app.MapHub<InfoHub>("/InfoHub");



app.Run();
