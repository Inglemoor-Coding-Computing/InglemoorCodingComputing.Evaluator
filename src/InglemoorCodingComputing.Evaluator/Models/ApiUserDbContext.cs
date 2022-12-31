namespace InglemoorCodingComputing.Evaluator.Models;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// EFCore DbContext for storing ApiUsers.
/// </summary>
public sealed class ApiUserDbContext : DbContext
{
    /// <summary>
    /// ApiUsers.
    /// </summary>
    public DbSet<ApiUser> ApiUsers { get; set; } = default!;

    private readonly string dbPath;
    
    /// <inheritdoc/>
    public ApiUserDbContext()
    {
        var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InglemoorCodingComputing.Evaluator");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        dbPath = Path.Join(path, "apiusers.db");
    }

    /// <summary>
    /// Use sqlite for database.
    /// </summary>
    /// <param name="options"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder options) => 
        options.UseSqlite($"Data Source={dbPath}");
}
