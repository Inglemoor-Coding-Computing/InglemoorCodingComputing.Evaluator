namespace InglemoorCodingComputing.Evaluator.Services;

using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public sealed class RunnerService : IRunnerService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Creates a new runner service.
    /// </summary>
    public RunnerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> AvailablePackagesAsync(Runner runner)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<string>>($"{runner.Endpoint}/available?key={runner.Key}");
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> LanguagesAsync(Runner runner)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<string>>($"{runner.Endpoint}/languages?key={runner.Key}");
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    /// <inheritdoc />
    public async Task<PingResponse?> PingAsync(Runner runner)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<PingResponse>($"{runner.Endpoint}/ping?key={runner.Key}");
            ArgumentNullException.ThrowIfNull(result);
            using RunnerDbContext context = new();
            var entity = await context.Runners.FindAsync(runner.Id);
            if (entity is not null)
                context.Entry(entity).CurrentValues.SetValues(runner with { Spec = result.Spec });
            await context.SaveChangesAsync();
            Updated?.Invoke();
            return result;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task UpdateOrAddAsync(Runner runner)
    {
        using RunnerDbContext context = new();
        var entity = await context.Runners.FindAsync(runner.Id);
        if (entity is null)
            await context.AddAsync(runner);
        else
            context.Entry(entity).CurrentValues.SetValues(runner);
        await context.SaveChangesAsync();
        Updated?.Invoke();
    }

    /// <inheritdoc />
    public async Task<string> SpecAsync(Runner runner, IReadOnlyList<(string, string)> spec)
    {
        var result = await _httpClient.PostAsJsonAsync($"{runner.Endpoint}/spec?key={runner.Key}", spec.Select(x => $"{x.Item1}={x.Item2}"));
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadAsStringAsync();
    }

    /// <inheritdoc />
    public Runner RandomRunner()
    {
        using RunnerDbContext context = new();
        var runners = context.Runners.AsNoTracking().ToArray();
        return runners[Random.Shared.Next(0, runners.Length)];
    }

    /// <inheritdoc />
    public IReadOnlyList<Runner> Runners()
    {
        using RunnerDbContext context = new();
        return context.Runners.ToArray();
    }

    /// <inheritdoc />
    public async Task AddRunner(Runner runner)
    {
        using RunnerDbContext context = new();
        await context.Runners.AddAsync(runner);
        await context.SaveChangesAsync();
        Updated?.Invoke();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Runner runner)
    {
        using RunnerDbContext context = new();
        var entity = await context.Runners.FindAsync(runner.Id);
        if (entity is null)
            return;
        context.Remove<Runner>(entity);
        await context.SaveChangesAsync();
        Updated?.Invoke();
    }

    /// <summary>
    /// On updated.
    /// </summary>
    public event Action? Updated;
}