using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Admin;

/// <summary>
/// Data transfer object for creating an administrator.
/// </summary>
public class CreateAdminDto
{
    /// <summary>
    /// The administrator's email address.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The administrator's password.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the administrator is deleted.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// The administrator's contact email address.
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The administrator's contact phone number.
    /// </summary>
    [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Phone(ErrorMessage = "Invalid {0} format")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The administrator's first name.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The administrator's last name.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The administrator's level (SuperAdmin or Moderator).
    /// </summary>
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Patronymic { get; set; }
}
