using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

/// <summary>
/// Data transfer object for creating a new employer.
/// </summary>
public class CreateEmployerDto
{
    /// <summary>
    /// The employer's email address.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The employer's password.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The employer's contact email address.
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The employer's contact phone number.
    /// </summary>
    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The employer's name.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The employer's description.
    /// </summary>
    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }

    /// <summary>
    /// The employer's website URL.
    /// </summary>
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Url(ErrorMessage = "Invalid {0} format")]
    public string? Website { get; set; }

    /// <summary>
    /// The employer's specialization.
    /// </summary>
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Specialization { get; set; }

    /// <summary>
    /// Indicates whether the employer is accredited.
    /// </summary>
    public bool AccreditationStatus { get; set; } = false;
}
