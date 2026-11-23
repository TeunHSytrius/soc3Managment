using Microsoft.AspNetCore.Mvc;
using Soc.Business.DTOs;
using Soc.Business.Services;

namespace Soc.Api.Controllers;

/// <summary>
/// Admin endpoints for managing customers (tenants) of the SOC.
/// </summary>
[ApiController]
[Route("api/v1/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Creates a new customer. At least a name is required.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _customerService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Lists all customers, including deactivated ones.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _customerService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets a single customer by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Deactivates a customer. Existing data is preserved.
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<CustomerResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.DeactivateAsync(id, cancellationToken);
        return Ok(response);
    }
}
