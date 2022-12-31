namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary>
/// Executes code.
/// </summary>
public interface IAsyncCodeExecutionService
{
    /// <summary>
    /// Executes code.
    /// </summary>
    /// <param name="request">Code execution request</param>
    /// <returns>Code execution result</returns>
    Task<CodeExecutionResult> ExecuteAsync(CodeExecutionRequest request);
}
