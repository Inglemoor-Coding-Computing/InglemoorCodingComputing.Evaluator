namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Api consumer.
/// </summary>
public sealed record ApiUser
{
    /// <summary>
    /// Api user id.
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Name of the api User.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Creation time of the api user.
    /// </summary>
    public required DateTime Creation { get; set; }
}
