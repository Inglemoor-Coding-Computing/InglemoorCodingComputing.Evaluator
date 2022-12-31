namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Sent to the evaluator for execution.
/// </summary>
public sealed record EvaluationResultRequest
{
    /// <summary>
    /// User Id for user that requested.
    /// </summary>
    public required Guid User { get; init; }

    /// <summary>
    /// Language Selected.
    /// </summary>
    public required string Language { get; init; }
    
    /// <summary>
    /// Code to execute.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Standard in.
    /// </summary>
    public required string StandardInput { get; set; }
}