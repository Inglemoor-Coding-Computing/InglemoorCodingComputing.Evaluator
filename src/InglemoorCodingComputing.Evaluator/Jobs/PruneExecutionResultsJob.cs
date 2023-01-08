namespace InglemoorCodingComputing.Evaluator.Jobs;

using Quartz;

/// <summary>
/// Prunes execution log.
/// </summary>
public class PruneExecutionResultsJob : IJob
{
    /// <summary>
    /// Deletes entries older than 24 hours.
    /// </summary>
    public async Task Execute(IJobExecutionContext _)
    {
        using ExecutionResultDbContext context = new();
        var threshold = DateTime.UtcNow.AddHours(-24);
        var delete = context.ExecutionResults.Where(x => x.Time < threshold);
        context.ExecutionResults.RemoveRange(delete);
        await context.SaveChangesAsync();
    }
}