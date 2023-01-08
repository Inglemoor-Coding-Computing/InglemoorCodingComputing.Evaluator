namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Api consumer.
/// </summary>
public sealed record ApiUser
{
    /// <summary>
    /// Api user id.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Name of the api User.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Creation time of the api user.
    /// </summary>
    public required DateTime Creation { get; init; }
}
