namespace InglemoorCodingComputing.Evaluator.Shared;

/// <summary>
/// Sent from the evaluator a execution runner
/// </summary>
public sealed record CodeExecutionRequest
{
    /// <summary>
    /// Generated id.
    /// </summary>
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Language used
    /// </summary>
    public required string Language { get; init; }
    
    /// <summary>
    /// Code.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Standard Input.
    /// </summary>
    public required string StandardIn { get; init; }
}