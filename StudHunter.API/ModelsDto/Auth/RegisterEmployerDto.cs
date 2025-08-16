using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Auth;

/// <summary>
/// Data transfer object for registering an employer.
/// </summary>
public class RegisterEmployerDto
{
    /// <summary>
    /// The employer's email address.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// The employer's password.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// The employer's name.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The employer's contact phone number (optional).
    /// </summary>
    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }
}
