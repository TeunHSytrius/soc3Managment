using Soc.Business.Models;

namespace Soc.Business.Repositories;

/// <summary>
/// Data access contract for the two-factor state of users.
/// </summary>
public interface ITwoFactorRepository
{
    Task<UserTwoFactor?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task SaveAsync(UserTwoFactor userTwoFactor, CancellationToken cancellationToken = default);
}
