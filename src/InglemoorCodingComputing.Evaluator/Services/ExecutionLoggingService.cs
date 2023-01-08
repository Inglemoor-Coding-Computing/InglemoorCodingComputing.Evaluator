using Microsoft.EntityFrameworkCore;

namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary> 
/// Stores logs of executions for a time.
/// </summary>
public sealed class ExecutionLoggingService : IExecutionLoggingService
{
    private readonly ExecutionResultDbContext _context;

    /// <summary>
    /// Creates a new ExecutionLogginService
    /// </summary>
    /// <param name="context"></param>
    public ExecutionLoggingService(ExecutionResultDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task LogSuccessAsync(Guid application, Guid user, Guid? instance, Guid id, TimeSpan duration)
    {
        await _context.ExecutionResults.AddAsync(new()
        {
            Id = id,
            Application = application,
            User = user,
            Instance = instance,
            Duration = duration,
            Time = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task LogFailureAsync(Guid application, Guid user, Guid? instance, Guid id, TimeSpan duration, string message)
    {
        await _context.ExecutionResults.AddAsync(new()
        {
            Id = id,
            Application = application,
            User = user,
            Instance = instance,
            Duration = duration,
            Time = DateTime.UtcNow,
            ErrorMessage = message
        });
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public Task<PaginatedList<ExecutionResult>> GetExecutionsAsync(int page, int size) =>
        PaginatedList<ExecutionResult>.CreateAsync(_context.ExecutionResults.AsNoTracking().OrderByDescending(x => x.Time), page, size);
}