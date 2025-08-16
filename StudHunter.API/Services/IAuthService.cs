using StudHunter.API.ModelsDto.Auth;

namespace StudHunter.API.Services;

public interface IAuthService
{
    Task<(AuthResultDto? Result, int? StatusCode, string? ErrorMessage)> LoginAsync(LoginDto dto);

    string GenerateJwtToken(Guid userId, string role);
}
