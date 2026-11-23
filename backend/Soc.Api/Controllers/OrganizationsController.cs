using Microsoft.AspNetCore.Mvc;
using Soc.Business.DTOs;
using Soc.Business.Services;

namespace Soc.Api.Controllers;

/// <summary>
/// Admin endpoints for managing tenant organisations of the SOC.
/// </summary>
[ApiController]
[Route("api/v1/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    /// <summary>
    /// Creates a new tenant organisation. At least a name is required.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrganizationResponse>> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var response = await _organizationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Lists all tenant organisations, including deactivated ones.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _organizationService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets a single tenant organisation by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrganizationResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _organizationService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Deactivates a tenant organisation. Existing data is preserved.
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<OrganizationResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var response = await _organizationService.DeactivateAsync(id, cancellationToken);
        return Ok(response);
    }
}
