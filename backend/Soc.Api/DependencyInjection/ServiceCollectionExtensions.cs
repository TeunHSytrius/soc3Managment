using Microsoft.Extensions.DependencyInjection;
using Soc.Api.ExceptionHandling;
using Soc.Business.Repositories;
using Soc.Business.Services;
using Soc.Data.Repositories;

namespace Soc.Api.DependencyInjection;

/// <summary>
/// Registers the organization management feature (US-20 / FR-16), including the
/// global exception handler and the in-memory repository stand-in. Keeping this
/// in an extension method isolates the registration from other feature branches
/// that configure their own services in Program.cs.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrganizationManagement(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // In-memory stand-in while the SQL Server data layer is developed separately;
        // swap InMemoryOrganizationRepository for the EF Core implementation once it exists.
        services.AddScoped<IOrganizationRepository, InMemoryOrganizationRepository>();
        services.AddScoped<IOrganizationService, OrganizationService>();

        return services;
    }
}
