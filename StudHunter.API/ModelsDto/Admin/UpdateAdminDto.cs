using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Admin;

/// <summary>
/// Data transfer object for updating an administrator.
/// </summary>
public class UpdateAdminDto
{
    /// <summary>
    /// The administrator's email address.
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? Email { get; set; }

    /// <summary>
    /// The administrator's password.
    /// </summary>
    [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string? Password { get; set; }

    /// <summary>
    /// Indicates whether the administrator is deleted.
    /// </summary>
    public bool? IsDeleted { get; set; }

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
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? FirstName { get; set; }

    /// <summary>
    /// The administrator's last name.
    /// </summary>
    [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? LastName { get; set; }

    /// <summary>
    /// The administrator's level (SuperAdmin or Moderator).
    /// </summary>
    [RegularExpression("SuperAdmin|Moderator", ErrorMessage = "{0} must be 'SuperAdmin' or 'Moderator'")]
    public string? AdminLevel { get; set; }
}
