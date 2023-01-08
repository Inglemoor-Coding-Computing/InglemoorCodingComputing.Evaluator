namespace InglemoorCodingComputing.Evaluator.Models;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// EFCore DbContext for storing runner endpoints.
/// </summary>
public sealed class RunnerDbContext : DbContext
{
    /// <summary>
    /// Execution Results.
    /// </summary>
    public DbSet<Runner> Runners { get; set; } = default!;

    private readonly string dbPath;

    /// <inheritdoc/>
    public RunnerDbContext()
    {
        var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InglemoorCodingComputing.Evaluator");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        dbPath = Path.Join(path, "runners.db");
    }

    /// <summary>
    /// Use sqlite for database.
    /// </summary>
    /// <param name="options"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlite($"Data Source={dbPath}");
}
