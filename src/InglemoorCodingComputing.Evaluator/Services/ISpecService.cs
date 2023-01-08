namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary>
/// Stores package specification.
/// </summary>
public interface ISpecService
{
    /// <summary>
    /// Package specification.
    /// </summary>
    IReadOnlyList<(string, string)> Spec { get; set; }
}