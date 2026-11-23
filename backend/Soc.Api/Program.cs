using Soc.Api.ExceptionHandling;
using Soc.Business.Repositories;
using Soc.Business.Services;
using Soc.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Organization management (US-20 / FR-16).
// The repository is an in-memory stand-in while the SQL Server data layer is
// developed separately; swap InMemoryOrganizationRepository for the EF Core
// implementation once it is available.
builder.Services.AddScoped<IOrganizationRepository, InMemoryOrganizationRepository>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
