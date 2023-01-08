namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Represents an execution runner.
/// </summary>
public sealed record Runner
{
    /// <summary>
    /// Id.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Name of the runner endpoint.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Endpoint.
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// Key.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Status of the endpoint.
    /// </summary>
    public required bool Enabled { get; init; }

    /// <summary>
    /// Last known specification.
    /// </summary>
    public string Spec { get; init; } = string.Empty;

    /// <summary>
    /// Whether this was specified through environment, non editable.
    /// </summary>
    public bool FromConfig { get; init; }
}