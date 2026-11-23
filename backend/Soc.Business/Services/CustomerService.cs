using Soc.Business.DTOs;
using Soc.Business.Exceptions;
using Soc.Business.Models;
using Soc.Business.Repositories;

namespace Soc.Business.Services;

/// <inheritdoc />
public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Een klantnaam is verplicht.");
        }

        var customer = Customer.Create(name!);
        await _repository.AddAsync(customer, cancellationToken);

        return ToResponse(customer);
    }

    public async Task<CustomerResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(id, cancellationToken);

        if (customer.IsActive)
        {
            customer.Deactivate();
            await _repository.UpdateAsync(customer, cancellationToken);
        }

        return ToResponse(customer);
    }

    public async Task<CustomerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(id, cancellationToken);
        return ToResponse(customer);
    }

    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _repository.GetAllAsync(cancellationToken);
        return customers.Select(ToResponse).ToList();
    }

    public async Task EnsureCanReceiveAlertsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(customerId, cancellationToken);
        if (!customer.IsActive)
        {
            throw new CustomerInactiveException(customer.Id);
        }
    }

    private async Task<Customer> GetCustomerOrThrowAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);
    }

    private static CustomerResponse ToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            DeactivatedAt = customer.DeactivatedAt,
        };
    }
}
