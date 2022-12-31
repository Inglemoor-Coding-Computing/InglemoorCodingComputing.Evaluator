namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Sent to the evaluator for execution and verification.
/// </summary>
public sealed record EvaluationVerificationRequest
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
    /// User code to execute.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Standard inputs.
    /// </summary>
    public required IReadOnlyList<string> Inputs { get; init; }
    
    /// <summary>
    /// Expected standard outputs, must match in length with inputs.
    /// </summary>
    public required IReadOnlyList<string> Outputs { get; init; }
}