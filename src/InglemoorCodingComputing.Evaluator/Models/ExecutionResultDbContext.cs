namespace InglemoorCodingComputing.Evaluator.Models;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// EFCore DbContext for storing ApiUsers.
/// </summary>
public sealed class ExecutionResultDbContext : DbContext
{
    /// <summary>
    /// Execution Results.
    /// </summary>
    public DbSet<ExecutionResult> ExecutionResults { get; set; } = default!;

    private readonly string dbPath;

    /// <inheritdoc/>
    public ExecutionResultDbContext()
    {
        var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InglemoorCodingComputing.Evaluator");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        dbPath = Path.Join(path, "executionresults.db");
    }

    /// <summary>
    /// Use sqlite for database.
    /// </summary>
    /// <param name="options"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlite($"Data Source={dbPath}");
}
