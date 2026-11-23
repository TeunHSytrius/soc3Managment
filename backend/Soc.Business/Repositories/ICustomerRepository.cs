using Soc.Business.Models;

namespace Soc.Business.Repositories;

/// <summary>
/// Data access contract for customers. Lives in the business layer so that the
/// data layer can provide an EF Core implementation and tests can provide an
/// in-memory implementation, without the business layer depending on the data layer.
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
}
