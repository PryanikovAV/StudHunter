using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.LoginRequest;

public class LoginRequestDto
{
    [Required(ErrorMessage = "{0} is required")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string Password { get; set; } = string.Empty;
}
