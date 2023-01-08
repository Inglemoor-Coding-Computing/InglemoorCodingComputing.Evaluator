using InglemoorCodingComputing.Evaluator.ExecutionRunner;
using InglemoorCodingComputing.Evaluator.Shared;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/ping", async ([FromQuery(Name = "key")] string? key, IConfiguration config, HttpClient httpClient) =>
{
    if (key != config["Key"]) return Results.Unauthorized();
    var spec = await httpClient.GetFromJsonAsync<IReadOnlyList<Package>>(config["Endpoint"] + "/api/v2/packages");
    spec ??= Array.Empty<Package>();
    return Results.Ok(new PingResponse
    {
        Instance = Instance.GetCurrent(),
        Date = DateTime.UtcNow,
        Spec = string.Join('\n', spec.Where(x => x.Installed).Select(x => $"{x.Name}={x.Version}"))
    });
});

app.MapGet("/available", async ([FromQuery(Name = "key")] string? key, IConfiguration config, HttpClient httpClient) =>
{
    if (key != config["Key"]) return Results.Unauthorized();
    var packages = await httpClient.GetFromJsonAsync<IReadOnlyList<Package>>(config["Endpoint"] + "/api/v2/packages");
    packages ??= Array.Empty<Package>();
    return Results.Ok(packages.Select(x => $"{x.Name}={x.Version}"));
});

app.MapGet("/languages", async ([FromQuery(Name = "key")] string? key, IConfiguration config, HttpClient httpClient) =>
{
    if (key != config["Key"]) return Results.Unauthorized();
    var languages = await httpClient.GetFromJsonAsync<IReadOnlyList<Language>>(config["Endpoint"] + "/api/v2/runtimes");
    languages ??= Array.Empty<Language>();
    return Results.Ok(languages.Select(x => $"{x.Name}={x.Version}"));
});

app.MapPost("/spec", async ([FromQuery(Name = "key")] string? key, IConfiguration config, [FromBody] IReadOnlyList<string> spec, HttpClient httpClient) =>
{
    if (key != config["Key"]) return Results.Unauthorized();
    var lines = spec.Where(x => !string.IsNullOrWhiteSpace(x));
    if (!lines.All(x => x.Split('=').Length == 2))
        return Results.BadRequest("Bad Spec");
    var oldSpec = (await httpClient.GetFromJsonAsync<IReadOnlyList<Package>>(config["Endpoint"] + "/api/v2/packages") ?? Array.Empty<Package>()).Where(x => x.Installed).Select(x => $"{x.Name}={x.Version}").Where(x => !string.IsNullOrWhiteSpace(x));

    HashSet<string> removed = new(oldSpec);
    removed.ExceptWith(lines);
    List<string> response = new();
    // run the uninstalls
    foreach (var package in removed)
    {
        var tokens = package.Split('=');
        var name = tokens[0];
        var version = tokens[1];
        using HttpRequestMessage message = new()
        {
            Method = HttpMethod.Delete,
            RequestUri = new(config["Endpoint"] + "/api/v2/packages"),
            Content = new StringContent($$"""{ "language": "{{name}}", "version": "{{version}}"}""", System.Text.Encoding.UTF8, "application/json")
        };
        Console.WriteLine(message.Content);
        var resp = await httpClient.SendAsync(message);
        if (resp.IsSuccessStatusCode)
            response.Add($"Removed {name}={version}");
        else
            response.Add($"ERROR: Could not remove {name}={version}; {await resp.Content.ReadAsStringAsync()}");
    }

    HashSet<string> installed = new(lines);
    installed.ExceptWith(oldSpec);
    foreach (var package in installed)
    {
        var tokens = package.Split('=');
        var name = tokens[0];
        var version = tokens[1];
        using HttpRequestMessage message = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new(config["Endpoint"] + "/api/v2/packages"),
            Content = new StringContent($$"""{ "language": "{{name}}", "version": "{{version}}"}""", System.Text.Encoding.UTF8, "application/json")

        };
        var resp = await httpClient.SendAsync(message);
        if (resp.IsSuccessStatusCode)
            response.Add($"Installed {name}={version}");
        else
            response.Add($"ERROR: Could not install {name}={version}; {await resp.Content.ReadAsStringAsync()}");
    }
    return Results.Ok(response.Count() != 0
    ? string.Join('\n', response)
    : "Already to spec");
});

app.MapPost("/execute", async ([FromBody] CodeExecutionRequest request, [FromServices] HttpClient httpClient, [FromQuery(Name = "key")] string? key, IConfiguration config) =>
{
    if (key != config["Key"]) return Results.Unauthorized();
    // Start timings.
    var start = DateTime.UtcNow;

    // Find the language and version needed for Piston.
    var tokens = request.Language.Split("=");

    // Call piston instance
    var res = await httpClient.PostAsJsonAsync(app.Configuration["Endpoint"] + "/api/v2/execute", new PistonRequest()
    {
        Language = tokens[0],
        Version = tokens[1],
        Files = new[]
        {
            new PistonRequest.File()
            {
                Content = request.Content
            }
        },
        Stdin = request.StandardIn
    });
    Console.WriteLine(await res.Content.ReadAsStringAsync());
    res.EnsureSuccessStatusCode();

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    var response = JsonSerializer.Deserialize<PistonResult>(await res.Content.ReadAsStringAsync(), options);
    ArgumentNullException.ThrowIfNull(response);

    var end = DateTime.UtcNow;

    // Return result
    return Results.Ok(new CodeExecutionResult
    {
        Id = request.Id,
        Instance = Instance.GetCurrent(),
        Code = response.Run.Code,
        Duration = end - start,
        Result = response.Run.Output,
        InstanceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty
    });
})
.WithName("Piston Execution")
.WithOpenApi();

app.Run();

internal static class Instance
{
    private static Guid? instance;

    /// <summary>
    /// Unique Identifier for an instance.
    /// </summary>
    internal static Guid GetCurrent()
    {
        if (instance.HasValue)
            return instance.Value;

        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InglemoorCodingComputing.Evaluator.ExecutionRunner");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "instance");

        if (File.Exists(path))
        {
            instance = Guid.Parse(File.ReadAllText(path));
            return instance.Value;
        }

        instance = Guid.NewGuid();
        File.WriteAllText(path, instance.Value.ToString());
        return instance.Value;
    }
}
internal record Package
{
    [JsonPropertyName("language")]
    public required string Name { get; init; }

    [JsonPropertyName("language_version")]
    public required string Version { get; init; }

    [JsonPropertyName("installed")]
    public required bool Installed { get; init; }
}

internal record Language
{
    [JsonPropertyName("language")]
    public required string Name { get; init; }

    [JsonPropertyName("version")]
    public required string Version { get; init; }

    [JsonPropertyName("runtime")]
    public string? Runtime { get; init; }
}