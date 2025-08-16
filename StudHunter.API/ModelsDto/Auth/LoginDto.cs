using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Auth;

/// <summary>
/// Data transfer object for a login request.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's password.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string Password { get; set; } = string.Empty;
}
