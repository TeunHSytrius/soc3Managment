using Microsoft.AspNetCore.Mvc;
using Soc.Business.DTOs;
using Soc.Business.Exceptions;
using Soc.Business.Services;

namespace Soc.Api.Controllers;

/// <summary>
/// Endpoints for setting up two-factor authentication with an authenticator app (US-25).
/// </summary>
[ApiController]
[Route("api/v1/auth/2fa")]
public sealed class TwoFactorController : ControllerBase
{
    private readonly ITwoFactorService _twoFactorService;

    public TwoFactorController(ITwoFactorService twoFactorService)
    {
        _twoFactorService = twoFactorService;
    }

    /// <summary>
    /// Starts the 2FA setup and returns the data for the QR code.
    /// </summary>
    [HttpPost("setup")]
    public async Task<ActionResult<TwoFactorSetupResponse>> StartSetup([FromBody] StartTwoFactorSetupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _twoFactorService.StartSetupAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (TwoFactorAlreadyEnabledException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
