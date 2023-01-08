namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary>
/// Service for interacting with runners.
/// </summary>
public interface IRunnerService
{
    /// <summary>
    /// Pings a runner endpoint
    /// </summary>
    /// <param name="runner">Runner to ping</param>
    /// <returns>Response</returns>
    Task<PingResponse?> PingAsync(Runner runner);

    /// <summary>
    /// Pings a runner for installed languages
    /// </summary>
    /// <param name="runner">Runner to ping</param>
    /// <returns>List of installed languages</returns>
    Task<IReadOnlyList<string>> LanguagesAsync(Runner runner);

    /// <summary>
    /// Get the available packages.
    /// </summary>
    /// <param name="runner"></param>
    /// <returns>Packages that could be installed</returns>
    Task<IReadOnlyList<string>> AvailablePackagesAsync(Runner runner);

    /// <summary>
    /// Sets specification for a runner.
    /// </summary>
    /// <param name="runner">Runner to set spec on</param>
    /// <param name="spec">key value pairs of language names and versions</param>
    /// <returns>Response</returns>
    Task<string> SpecAsync(Runner runner, IReadOnlyList<(string, string)> spec);

    /// <summary>
    /// Get a random enabled execution runner.
    /// </summary>
    /// <returns>Random runner</returns>
    Runner RandomRunner();

    /// <summary>
    /// Gets all runners.
    /// </summary>
    /// <returns>Runners</returns>
    IReadOnlyList<Runner> Runners();

    /// <summary>
    /// Adds a new runner.
    /// </summary>
    /// <param name="runner">runner to add</param>
    Task AddRunner(Runner runner);

    /// <summary>
    /// Updates a runner.
    /// </summary>
    /// <param name="runner"></param>
    /// <returns></returns>
    Task UpdateOrAddAsync(Runner runner);

    /// <summary>
    /// Deletes a runner.
    /// </summary>
    /// <param name="runner"></param>
    /// <returns></returns>
    Task DeleteAsync(Runner runner);

    /// <summary>
    /// Fired on changes to the runners.
    /// </summary>
    event Action Updated;
}