using Soc.Business.Models;
using Soc.Business.Repositories;

namespace Soc.Data.Repositories;

/// <summary>
/// In-memory stand-in for the tenant organisation repository. Used while the
/// SQL Server data layer (EF Core / per-customer schema) is being developed
/// separately. The data layer implementation should replace this registration
/// in the API.
/// </summary>
public sealed class InMemoryOrganizationRepository : IOrganizationRepository
{
    private readonly Dictionary<Guid, Organization> _organizations = new();
    private readonly object _lock = new();

    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_organizations.TryGetValue(id, out var organization) ? organization : null);
        }
    }

    public Task<IReadOnlyList<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<Organization> organizations = _organizations.Values.ToList();
            return Task.FromResult(organizations);
        }
    }

    public Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _organizations[organization.Id] = organization;
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _organizations[organization.Id] = organization;
        }

        return Task.CompletedTask;
    }
}
