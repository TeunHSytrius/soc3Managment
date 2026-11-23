using Soc.Business.Models;
using Soc.Business.Repositories;

namespace Soc.Data.Repositories;

/// <summary>
/// In-memory stand-in for the customer repository. Used while the SQL Server
/// data layer (EF Core / per-customer schema) is being developed separately.
/// The data layer implementation should replace this registration in the API.
/// </summary>
public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<Guid, Customer> _customers = new();
    private readonly object _lock = new();

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_customers.TryGetValue(id, out var customer) ? customer : null);
        }
    }

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<Customer> customers = _customers.Values.ToList();
            return Task.FromResult(customers);
        }
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _customers[customer.Id] = customer;
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _customers[customer.Id] = customer;
        }

        return Task.CompletedTask;
    }
}
