using Microsoft.EntityFrameworkCore;
using Soc.Data.Context;


// variabelen inladen vanaf de .env
DotNetEnv.Env.Load(
    Path.Combine(Directory.GetCurrentDirectory(), ".env")
);

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
                          .GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException(
                          "Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<SocDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
