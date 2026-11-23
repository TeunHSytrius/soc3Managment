using Microsoft.AspNetCore.DataProtection;
using Soc.Business.Security;

namespace Soc.Api.Security;

/// <summary>
/// Encrypts secrets with ASP.NET Core Data Protection.
/// </summary>
public sealed class DataProtectionSecretProtector : ISecretProtector
{
    // A separate purpose ensures these keys can only decrypt 2FA secrets.
    private const string Purpose = "Soc.TwoFactor.Secret.v1";

    private readonly IDataProtector _protector;

    public DataProtectionSecretProtector(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public string Protect(string plainText) => _protector.Protect(plainText);

    public string Unprotect(string protectedText) => _protector.Unprotect(protectedText);
}
