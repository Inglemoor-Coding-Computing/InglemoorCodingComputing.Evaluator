namespace InglemoorCodingComputing.Evaluator.Models;

/// <summary>
/// Batched evaluation verification response.
/// </summary>
public sealed record EvaluationVerificationResponse
{
    /// <summary>
    /// 0 for success.
    /// </summary>
    public required IReadOnlyList<int> StatusCodes { get; set; }

    /// <summary>
    /// True for verification success.
    /// </summary>
    public required IReadOnlyList<bool> Results { get; set; }

    /// <summary>
    /// Standard outputs.
    /// </summary>
    public required IReadOnlyList<string> Outputs { get; set; }
}
