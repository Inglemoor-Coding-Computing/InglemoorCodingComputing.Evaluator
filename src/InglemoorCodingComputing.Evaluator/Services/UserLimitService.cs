namespace InglemoorCodingComputing.Evaluator.Services;

using System.Collections.Concurrent;

/// <inheritdoc/>
public class UserLimitService : IUserLimitService
{
    private readonly ConcurrentDictionary<Guid, DateTime> locks = new();

    /// <inheritdoc/>
    public void Release(Guid id) =>
        locks.TryRemove(id, out var _);

    /// <inheritdoc/>
    public bool TryLock(Guid id)
    {
        if (!locks.TryGetValue(id, out var time))
        {
            locks.TryAdd(id, DateTime.UtcNow);
            return true;
        }
        if (DateTime.UtcNow - time > TimeSpan.FromSeconds(10))
        {
            locks[id] = DateTime.UtcNow;
            return true;
        }
        else
            return false;
    }
}
