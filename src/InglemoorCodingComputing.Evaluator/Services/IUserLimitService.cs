namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary>
/// Limits the number of users accessing something.
/// </summary>
public interface IUserLimitService
{
    /// <summary>
    /// Locks.
    /// </summary>
    /// <param name="id">User's id.</param>
    /// <returns>True if successful, false if already locked.</returns>
    bool TryLock(Guid id);

    /// <summary>
    /// Releases lock.
    /// </summary>
    /// <param name="id">User's id.</param>
    void Release(Guid id);
}
