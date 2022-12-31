namespace InglemoorCodingComputing.Evaluator.Shared;

/// <summary>
/// Returned from a execution runner to the evaluator.
/// </summary>
public sealed class CodeExecutionResult
{
    /// <summary>
    /// Execution identification.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The amount of time an execution took.
    /// </summary>
    public required TimeSpan Duration { get; init; }

    /// <summary>
    /// Runner instance 
    /// </summary>
    public required Guid Instance { get; init; }

    /// <summary>
    /// Exit code of the run.
    /// </summary>
    public required int Code { get; init; }
    
    /// <summary>
    /// Result of code execution.
    /// </summary>
    public required string Result { get; init; }

    /// <summary>
    /// Version of the code execution runner.
    /// </summary>
    public required string InstanceVersion { get; init; }
}