namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Log of an execution
/// </summary>
public sealed record ExecutionResult
{
    /// <summary>
    /// Execution Id.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Application Id
    /// </summary>
    public required Guid Application { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public required Guid User { get; set; }

    /// <summary>
    /// Instance Id, if available.
    /// </summary>
    public required Guid? Instance { get; set; }

    /// <summary>
    /// Duration of code execution.
    /// </summary>
    public required TimeSpan Duration { get; set; }

    /// <summary>
    /// Time of execution.
    /// </summary>
    public required DateTime Time { get; set; }

    /// <summary>
    /// Error Message, null if success.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Whether the execution errored.
    /// </summary>
    public bool Errored => ErrorMessage is not null;
}