using Soc.Business.Models;

namespace Soc.Business.Repositories;

/// <summary>
/// Data access contract for tenant organisations. Lives in the business layer so
/// that the data layer can provide an EF Core implementation and tests can provide
/// an in-memory implementation, without the business layer depending on the data layer.
/// </summary>
public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Organization>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);

    Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default);
}
