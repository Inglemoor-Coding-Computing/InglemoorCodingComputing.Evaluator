namespace InglemoorCodingComputing.Evaluator.Services;

using System.Text.Json;

/// <summary>
/// Executes code on a remote worker
/// </summary>
public sealed class RemoteCodeExecutionService : IAsyncCodeExecutionService
{
    private readonly HttpClient _httpClient;
    private readonly IRunnerService _runnerService;
    private readonly IConfiguration _configuration;

    /// <inheritdoc/>
    public RemoteCodeExecutionService(HttpClient httpClient, IConfiguration configuration, IRunnerService runnerService)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _runnerService = runnerService;
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc/>
    public async Task<CodeExecutionResult> ExecuteAsync(CodeExecutionRequest request)
    {
        var runner = _runnerService.RandomRunner();
        var res = await _httpClient.PostAsJsonAsync($"{runner.Endpoint}/execute?key={runner.Key}", request);
        res.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<CodeExecutionResult>(await res.Content.ReadAsStringAsync(), _jsonOptions)!;
    }
}
