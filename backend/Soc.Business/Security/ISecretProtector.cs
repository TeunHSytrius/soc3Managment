namespace Soc.Business.Security;

/// <summary>
/// Encrypts and decrypts secrets before they are stored. The API layer provides
/// an implementation based on ASP.NET Core Data Protection, so the business layer
/// does not depend on ASP.NET Core.
/// </summary>
public interface ISecretProtector
{
    string Protect(string plainText);

    string Unprotect(string protectedText);
}
