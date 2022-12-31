namespace InglemoorCodingComputing.Evaluator.Services;

/// <summary>
/// Gets and sets auth state.
/// </summary>
public class AuthenticationStateService
{
    /// <summary>
    /// Gets auth state
    /// </summary>
    public bool Authenticated =>
        authTime is not null &&
        (DateTime.UtcNow - authTime.Value) < TimeSpan.FromMinutes(10);

    private DateTime? authTime;

    private readonly string? _password;

    /// <summary>
    /// Try to set the authentication state
    /// </summary>
    public bool Authenticate(string password)
    {
        authTime = _password is not null && password == _password ? DateTime.UtcNow : null;
        return authTime is not null;
    }

    /// <summary>
    /// Creates a new auth state service.
    /// </summary>
    public AuthenticationStateService(IConfiguration configuration)
    {
        _password = configuration["Admin:Password"];
    }
}