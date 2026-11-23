using Soc.Business.DTOs;
using Soc.Business.Exceptions;
using Soc.Business.Models;
using Soc.Business.Repositories;

namespace Soc.Business.Services;

/// <inheritdoc />
public sealed class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repository;

    public OrganizationService(IOrganizationRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrganizationResponse> CreateAsync(CreateOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Een naam is verplicht.");
        }

        var organization = Organization.Create(name!);
        await _repository.AddAsync(organization, cancellationToken);

        return ToResponse(organization);
    }

    public async Task<OrganizationResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organization = await GetOrganizationOrThrowAsync(id, cancellationToken);

        if (organization.IsActive)
        {
            organization.Deactivate();
            await _repository.UpdateAsync(organization, cancellationToken);
        }

        return ToResponse(organization);
    }

    public async Task<OrganizationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organization = await GetOrganizationOrThrowAsync(id, cancellationToken);
        return ToResponse(organization);
    }

    public async Task<IReadOnlyList<OrganizationResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var organizations = await _repository.GetAllAsync(cancellationToken);
        return organizations.Select(ToResponse).ToList();
    }

    public async Task EnsureCanReceiveAlertsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var organization = await GetOrganizationOrThrowAsync(organizationId, cancellationToken);
        if (!organization.IsActive)
        {
            throw new OrganizationInactiveException(organization.Id);
        }
    }

    private async Task<Organization> GetOrganizationOrThrowAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new OrganizationNotFoundException(id);
    }

    private static OrganizationResponse ToResponse(Organization organization)
    {
        return new OrganizationResponse
        {
            Id = organization.Id,
            Name = organization.Name,
            IsActive = organization.IsActive,
            CreatedAt = organization.CreatedAt,
            DeactivatedAt = organization.DeactivatedAt,
        };
    }
}
