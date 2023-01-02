using InglemoorCodingComputing.Evaluator.ExecutionRunner;
using InglemoorCodingComputing.Evaluator.Shared;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

app.MapPost("/execute", async ([FromBody] CodeExecutionRequest request, [FromServices] HttpClient httpClient, [FromQuery(Name = "key")] string? key, IConfiguration config) =>
{
    if (key != config["Key"]) return Results.Unauthorized();
    // Start timings.
    var start = DateTime.UtcNow;

    // Find the language and version needed for Piston.
    var r = DecodeLanguage(request.Language);
    if (r is null)
        return Results.BadRequest("Invalid language selection.");

    // Call piston instance
    var res = await httpClient.PostAsJsonAsync(app.Configuration["Endpoint"] + "/api/v2/execute", new PistonRequest()
    {
        Language = r.Value.Item1,
        Version = r.Value.Item2,
        Files = new[]
        {
            new PistonRequest.File()
            {
                Content = request.Content
            }
        },
        Stdin = request.StandardIn
    });
    res.EnsureSuccessStatusCode();

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    var response = JsonSerializer.Deserialize<PistonResult>(await res.Content.ReadAsStringAsync(), options);
    ArgumentNullException.ThrowIfNull(response);

    var end = DateTime.UtcNow;

    // Return result
    return Results.Ok(new CodeExecutionResult()
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

static (string, string)? DecodeLanguage(string x) => x.ToLowerInvariant() switch
{
    "fs" => ("fsharp", "5.0.201"),
    "fsi" => ("fsi", "5.0.201"),
    "cs" => ("csharp", "5.0.201"),
    "py" => ("python", "3.10.0"),
    "java" => ("java", "15.0.2"),
    "cpp" => ("c++", "10.2.0"),
    "c" => ("c", "10.2.0"),
    _ => null
};

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