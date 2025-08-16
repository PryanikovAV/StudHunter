using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudHunter.API.ModelsDto.Auth;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudHunter.API.Services;

public class AuthService(StudHunterDbContext context, IPasswordHasher passwordHasher, IConfiguration configuration) : BaseService(context), IAuthService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="dto">The data transfer object containing login credentials.</param>
    /// <returns>A tuple containing the authentication result, an optional status code, and an optional error message.</returns>
    public async Task<(AuthResultDto? Result, int? StatusCode, string? ErrorMessage)> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);

        #region Serializers
        if (user == null || !passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            return (null, StatusCodes.Status401Unauthorized, "Incorrect login or password");
        #endregion

        var role = user is Student ? nameof(Student) : user is Employer ? nameof(Employer) : nameof(Administrator);

        var token = GenerateJwtToken(user.Id, role);

        return (new AuthResultDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = role,
            Token = token
        }, null, null);
    }

    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The role of the user (e.g., Student, Employer, Administrator).</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GenerateJwtToken(Guid userId, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("JWT Key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured.");

        var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
