namespace InglemoorCodingComputing.Evaluator.Services;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Admin service.
/// </summary>
public class AdminService
{
    private readonly IConfiguration _configuration;
    private readonly ApiUserDbContext _apiUserDbContext;

    /// <inheritdoc/>
    public AdminService(IConfiguration configuration, ApiUserDbContext apiUserDbContext)
    {
        _configuration = configuration;
        _apiUserDbContext = apiUserDbContext;
    }

    /// <summary>
    /// Returns all users.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ApiUser> GetUsers() =>
        _apiUserDbContext.Set<ApiUser>();

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiUser> CreateUserAsync(CreateUserRequest request)
    {
        ApiUser user = new() { Id = Guid.NewGuid(), Name = request.Name, Creation = DateTime.UtcNow };
        await _apiUserDbContext.AddAsync(user);
        await _apiUserDbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Tries to delete a user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>success</returns>
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _apiUserDbContext.FindAsync<ApiUser>(id);
        if (user is null)
            return false;
        _apiUserDbContext.Remove(user);
        await _apiUserDbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Attempts to generate an api key.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<LoginResponse?> GenerateApiTokenAsync(Guid id, int? days)
    {
        // Make sure the user actually exists.
        var user = await _apiUserDbContext.FindAsync<ApiUser>(id);
        if (user is null)
            return null;

        List<Claim> claims = new()
        {
            // Id is checked agains the database to ensure the user still exists.
            new Claim("nameid", id.ToString()),
        };

        // Generate JWT.
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new Exception("Set the JWT Secret.")));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken token = new(claims: claims, expires: days is null ? null : DateTime.UtcNow.AddDays(days.Value), signingCredentials: creds);
        return new(new JwtSecurityTokenHandler().WriteToken(token));
    }

    /// <summary>
    /// Request for login.
    /// </summary>
    /// <param name="Key">Login key.</param>
    public record LoginRequest(string Key);

    /// <summary>
    /// Response from successful login.
    /// </summary>
    /// <param name="Token">Access token.</param>
    public record LoginResponse(string Token);

    /// <summary>
    /// Request for creating a user.
    /// </summary>
    /// <param name="Name">Name of the user.</param>
    public record CreateUserRequest(string Name);
}
