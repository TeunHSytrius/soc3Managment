using Microsoft.Extensions.DependencyInjection.Extensions;
using Soc.Api.Security;
using Soc.Business.Repositories;
using Soc.Business.Security;
using Soc.Business.Services;
using Soc.Data.Repositories;

namespace Soc.Api.DependencyInjection;

/// <summary>
/// Registers two-factor authentication (US-25). Kept in its own extension method so
/// Program.cs only needs a single line and other feature branches do not conflict.
/// </summary>
public static class TwoFactorServiceCollectionExtensions
{
    public static IServiceCollection AddTwoFactorAuthentication(this IServiceCollection services)
    {
        // TODO: persist the Data Protection keys (e.g. in the database) before going to production;
        // otherwise stored secrets can no longer be decrypted after the keys are lost.
        services.AddDataProtection();
        services.AddSingleton<ISecretProtector, DataProtectionSecretProtector>();
        services.TryAddSingleton(TimeProvider.System);

        // Singleton because the in-memory store must survive between requests.
        // Replace with the EF Core repository (scoped) once the database is available.
        services.AddSingleton<ITwoFactorRepository, InMemoryTwoFactorRepository>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();

        return services;
    }
}
