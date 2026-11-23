using Soc.Business.DTOs;
using Soc.Business.Exceptions;
using Soc.Business.Services;
using Soc.Data.Repositories;

namespace Soc.Tests;

public class CustomerServiceTests
{
    private static CustomerService CreateService()
    {
        return new CustomerService(new InMemoryCustomerRepository());
    }

    [Fact]
    public async Task CreateAsync_WithValidName_CreatesActiveCustomer()
    {
        var service = CreateService();

        var response = await service.CreateAsync(new CreateCustomerRequest { Name = "Acme Bank" });

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Acme Bank", response.Name);
        Assert.True(response.IsActive);
        Assert.Null(response.DeactivatedAt);
    }

    [Fact]
    public async Task CreateAsync_TrimsSurroundingWhitespace()
    {
        var service = CreateService();

        var response = await service.CreateAsync(new CreateCustomerRequest { Name = "  Klant A  " });

        Assert.Equal("Klant A", response.Name);
    }

    [Fact]
    public async Task CreateAsync_WithWhitespaceName_ThrowsValidation()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new CreateCustomerRequest { Name = "   " }));
    }

    [Fact]
    public async Task CreateAsync_WithNullName_ThrowsValidation()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new CreateCustomerRequest { Name = null! }));
    }

    [Fact]
    public async Task DeactivateAsync_MarksInactiveAndPreservesExistingData()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        var created = await service.CreateAsync(new CreateCustomerRequest { Name = "Klant A" });

        var deactivated = await service.DeactivateAsync(created.Id);

        Assert.False(deactivated.IsActive);
        Assert.NotNull(deactivated.DeactivatedAt);

        // Existing data is preserved: the record is still retrievable with its name intact.
        var stored = await repository.GetByIdAsync(created.Id);
        Assert.NotNull(stored);
        Assert.False(stored!.IsActive);
        Assert.Equal("Klant A", stored.Name);
    }

    [Fact]
    public async Task DeactivateAsync_UnknownId_ThrowsNotFound()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => service.DeactivateAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeactivateAsync_AlreadyInactive_IsIdempotent()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateCustomerRequest { Name = "Klant B" });

        await service.DeactivateAsync(created.Id);
        var again = await service.DeactivateAsync(created.Id);

        Assert.False(again.IsActive);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveAndInactiveCustomers()
    {
        var service = CreateService();
        await service.CreateAsync(new CreateCustomerRequest { Name = "Actief" });
        var toDeactivate = await service.CreateAsync(new CreateCustomerRequest { Name = "Wordt inactief" });
        await service.DeactivateAsync(toDeactivate.Id);

        var customers = await service.GetAllAsync();

        Assert.Equal(2, customers.Count);
        Assert.Contains(customers, c => c.Name == "Actief" && c.IsActive);
        Assert.Contains(customers, c => c.Name == "Wordt inactief" && !c.IsActive);
    }

    [Fact]
    public async Task EnsureCanReceiveAlertsAsync_ActiveCustomer_DoesNotThrow()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateCustomerRequest { Name = "Klant C" });

        await service.EnsureCanReceiveAlertsAsync(created.Id);
    }

    [Fact]
    public async Task EnsureCanReceiveAlertsAsync_InactiveCustomer_ThrowsInactive()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateCustomerRequest { Name = "Klant D" });
        await service.DeactivateAsync(created.Id);

        await Assert.ThrowsAsync<CustomerInactiveException>(
            () => service.EnsureCanReceiveAlertsAsync(created.Id));
    }

    [Fact]
    public async Task EnsureCanReceiveAlertsAsync_UnknownCustomer_ThrowsNotFound()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => service.EnsureCanReceiveAlertsAsync(Guid.NewGuid()));
    }
}
