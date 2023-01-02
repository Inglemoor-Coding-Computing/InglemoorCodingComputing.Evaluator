namespace InglemoorCodingComputing.Evaluator.Services;

using System.Text.Json;

/// <summary>
/// Executes code on a remote worker
/// </summary>
public sealed class RemoteCodeExecutionService : IAsyncCodeExecutionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    /// <inheritdoc/>
    public RemoteCodeExecutionService(HttpClient httpClient, IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc/>
    public async Task<CodeExecutionResult> ExecuteAsync(CodeExecutionRequest request)
    {
        var res = await _httpClient.PostAsJsonAsync(_configuration["ExecutionEndpoint"], request);
        res.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<CodeExecutionResult>(await res.Content.ReadAsStringAsync(), _jsonOptions)!;
    }
}
