using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto;

public record AuthDto(Guid Id, string Email, string Role, string Token);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required, StringLength(100, MinimumLength = 8)] string Password);

public record RegisterStudentDto(
    [Required, EmailAddress] string Email,
    [Required, StringLength(255, MinimumLength = 8)] string Password);

public record RegisterEmployerDto(
    [Required, EmailAddress] string Email,
    [Required, StringLength(100, MinimumLength = 8)] string Password,
    [Required, StringLength(255)] string Name);

public record DeleteAccountDto(
    [Required] string Password
);