using System.Security.Cryptography;
using System.Text;

namespace Soc.Business.Security;

/// <summary>
/// Generates and hashes one-time recovery codes (format XXXX-XXXX).
/// </summary>
public static class RecoveryCodes
{
    public const int Count = 10;

    // No 0/O, 1/I/L, so codes are easy to read and type over.
    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 8;

    public static IReadOnlyList<string> Generate()
    {
        var codes = new List<string>(Count);

        for (int i = 0; i < Count; i++)
        {
            string code = RandomNumberGenerator.GetString(Alphabet, CodeLength);
            codes.Add($"{code[..4]}-{code[4..]}");
        }

        return codes;
    }

    /// <summary>
    /// Hashes a code after normalising it, so "abcd-efgh", "ABCDEFGH" and " ABCD-EFGH "
    /// all give the same hash. The codes are long and random, so plain SHA-256 is sufficient.
    /// </summary>
    public static string Hash(string code)
    {
        string normalized = Normalize(code);
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(hash);
    }

    public static bool LooksLikeRecoveryCode(string input)
    {
        return Normalize(input).Length == CodeLength;
    }

    private static string Normalize(string code)
    {
        return code.Replace("-", string.Empty).Replace(" ", string.Empty).Trim().ToUpperInvariant();
    }
}
