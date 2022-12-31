namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Returned from evaluation.
/// </summary>
public sealed record EvaluationResultResponse
{
    /// <summary>
    /// Exit code of process, 0 for success.
    /// </summary>
    public required int Code { get; init; }

    /// <summary>
    /// Standard output.
    /// </summary>
    public required string Output { get; init; }
}
