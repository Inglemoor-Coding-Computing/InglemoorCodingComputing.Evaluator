namespace InglemoorCodingComputing.Evaluator.Services;

using System.Text.Json;

/// <inheritdoc/>
public class SpecService : ISpecService
{
    private readonly string path;

    /// <summary>
    /// Creates a new SpecService.
    /// </summary>
    public SpecService()
    {
        var dir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InglemoorCodingComputing.Evaluator");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        path = Path.Join(dir, "spec.json");
    }

    /// <inheritdoc/>
    public IReadOnlyList<(string, string)> Spec
    {
        get =>
            File.Exists(path)
            ? File.ReadLines(path).Select(x =>
            {
                var tokens = x.Split('=');
                return (tokens[0], tokens[1]);
            }).ToList()
            : new List<(string, string)>();
        set =>
            File.WriteAllLines(path, value.Select(x => $"{x.Item1}={x.Item2}"));
    }
}