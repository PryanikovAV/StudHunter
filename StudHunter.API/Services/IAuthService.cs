using StudHunter.API.ModelsDto.AuthDto;

namespace StudHunter.API.Services;

public interface IAuthService
{
    Task<(AuthResultDto? Result, int? StatusCode, string? ErrorMessage)> LoginAsync(LoginDto dto);

    Task<(AuthResultDto? Result, int? StatusCode, string? ErrorMessage)> RecoverAccountAsync(LoginDto dto);

    string GenerateJwtToken(Guid userId, string role);
}
