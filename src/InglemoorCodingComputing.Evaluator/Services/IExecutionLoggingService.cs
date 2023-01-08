namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary> 
/// Stores logs of executions for a time.
/// </summary>
public interface IExecutionLoggingService
{
    /// <summary>
    /// Logs the success of a code execution.
    /// </summary>
    /// <param name="application">The client application</param>
    /// <param name="user">Client user</param>
    /// <param name="instance">The execution runner instance if available</param>
    /// <param name="id">Code execution id</param>
    /// <param name="duration">Duration of code execution</param>
    Task LogSuccessAsync(Guid application, Guid user, Guid? instance, Guid id, TimeSpan duration);

    /// <summary>
    /// Log the failure of a code execution.
    /// </summary>
    /// <param name="application">The client application</param>
    /// <param name="user">Client user</param>
    /// <param name="instance">The execution runner instance if available</param>
    /// <param name="id">Code execution id</param>
    /// <param name="duration">Duration of code execution/failure</param>
    /// <param name="message">Error message</param>
    Task LogFailureAsync(Guid application, Guid user, Guid? instance, Guid id, TimeSpan duration, string message);

    /// <summary>
    /// Executions.
    /// </summary>
    Task<PaginatedList<ExecutionResult>> GetExecutionsAsync(int page, int size);
}