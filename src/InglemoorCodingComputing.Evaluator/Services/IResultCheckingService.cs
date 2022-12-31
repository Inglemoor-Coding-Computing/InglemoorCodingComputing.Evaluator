namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary>
/// Verifies that an input and output match.
/// </summary>
public interface IResultCheckingService
{
    /// <summary>
    /// Verifies that an input and output match.
    /// </summary>
    /// <param name="expected">Expected set of lines.</param>
    /// <param name="generated">Generated set of lines to check.</param>
    /// <remarks>
    /// Expected and actual are both split into lines.
    /// Empty lines from start and end are removed.
    /// Remaining lines are compared.
    /// </remarks>
    /// <returns>True if they are the "same"</returns>
    bool Verify(string expected, string generated);
}
