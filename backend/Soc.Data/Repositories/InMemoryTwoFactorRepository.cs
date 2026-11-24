using Soc.Business.Models;
using Soc.Business.Repositories;

namespace Soc.Data.Repositories;

/// <summary>
/// In-memory stand-in for the two-factor repository while the SQL Server data layer
/// is developed separately. Data is lost when the API restarts.
/// </summary>
public sealed class InMemoryTwoFactorRepository : ITwoFactorRepository
{
    private readonly Dictionary<int, UserTwoFactor> _items = new();
    private readonly object _lock = new();

    public Task<UserTwoFactor?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_items.TryGetValue(userId, out var item) ? item : null);
        }
    }

    public Task SaveAsync(UserTwoFactor userTwoFactor, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _items[userTwoFactor.UserId] = userTwoFactor;
        }

        return Task.CompletedTask;
    }

    public Task<bool> TryConsumeRecoveryCodeAsync(int userId, string recoveryCodeHash, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            bool removed = _items.TryGetValue(userId, out var item)
                && item.RecoveryCodeHashes.Remove(recoveryCodeHash);

            return Task.FromResult(removed);
        }
    }
}
