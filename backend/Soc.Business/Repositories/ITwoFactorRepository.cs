using Soc.Business.Models;

namespace Soc.Business.Repositories;

/// <summary>
/// Data access contract for the two-factor state of users.
/// </summary>
public interface ITwoFactorRepository
{
    Task<UserTwoFactor?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task SaveAsync(UserTwoFactor userTwoFactor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the recovery code hash in one atomic step. Returns false if it did not
    /// exist (anymore), so two simultaneous requests can never both use the same code (#166).
    /// With a database this becomes a single DELETE and a check on the number of affected rows.
    /// </summary>
    Task<bool> TryConsumeRecoveryCodeAsync(int userId, string recoveryCodeHash, CancellationToken cancellationToken = default);
}
