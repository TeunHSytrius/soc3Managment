using Soc.Business.DTOs;
using Soc.Business.Exceptions;
using Soc.Business.Services;
using Soc.Data.Repositories;

namespace Soc.Tests;

public class OrganizationServiceTests
{
    private static OrganizationService CreateService()
    {
        return new OrganizationService(new InMemoryOrganizationRepository());
    }

    [Fact]
    public async Task CreateAsync_WithValidName_CreatesActiveOrganization()
    {
        var service = CreateService();

        var response = await service.CreateAsync(new CreateOrganizationRequest { Name = "Acme Bank" });

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Acme Bank", response.Name);
        Assert.True(response.IsActive);
        Assert.Null(response.DeactivatedAt);
    }

    [Fact]
    public async Task CreateAsync_TrimsSurroundingWhitespace()
    {
        var service = CreateService();

        var response = await service.CreateAsync(new CreateOrganizationRequest { Name = "  Klant A  " });

        Assert.Equal("Klant A", response.Name);
    }

    [Fact]
    public async Task CreateAsync_WithWhitespaceName_ThrowsValidation()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new CreateOrganizationRequest { Name = "   " }));
    }

    [Fact]
    public async Task CreateAsync_WithNullName_ThrowsValidation()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new CreateOrganizationRequest { Name = null! }));
    }

    [Fact]
    public async Task DeactivateAsync_MarksInactiveAndPreservesExistingData()
    {
        var repository = new InMemoryOrganizationRepository();
        var service = new OrganizationService(repository);

        var created = await service.CreateAsync(new CreateOrganizationRequest { Name = "Klant A" });

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

        await Assert.ThrowsAsync<OrganizationNotFoundException>(
            () => service.DeactivateAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeactivateAsync_AlreadyInactive_IsIdempotent()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrganizationRequest { Name = "Klant B" });

        await service.DeactivateAsync(created.Id);
        var again = await service.DeactivateAsync(created.Id);

        Assert.False(again.IsActive);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveAndInactiveOrganizations()
    {
        var service = CreateService();
        await service.CreateAsync(new CreateOrganizationRequest { Name = "Actief" });
        var toDeactivate = await service.CreateAsync(new CreateOrganizationRequest { Name = "Wordt inactief" });
        await service.DeactivateAsync(toDeactivate.Id);

        var organizations = await service.GetAllAsync();

        Assert.Equal(2, organizations.Count);
        Assert.Contains(organizations, c => c.Name == "Actief" && c.IsActive);
        Assert.Contains(organizations, c => c.Name == "Wordt inactief" && !c.IsActive);
    }

    [Fact]
    public async Task EnsureCanReceiveAlertsAsync_ActiveOrganization_DoesNotThrow()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrganizationRequest { Name = "Klant C" });

        await service.EnsureCanReceiveAlertsAsync(created.Id);
    }

    [Fact]
    public async Task EnsureCanReceiveAlertsAsync_InactiveOrganization_ThrowsInactive()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrganizationRequest { Name = "Klant D" });
        await service.DeactivateAsync(created.Id);

        await Assert.ThrowsAsync<OrganizationInactiveException>(
            () => service.EnsureCanReceiveAlertsAsync(created.Id));
    }

    [Fact]
    public async Task EnsureCanReceiveAlertsAsync_UnknownOrganization_ThrowsNotFound()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<OrganizationNotFoundException>(
            () => service.EnsureCanReceiveAlertsAsync(Guid.NewGuid()));
    }
}
