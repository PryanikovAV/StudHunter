using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Admin;

/// <summary>
/// Data transfer object for an administrator.
/// </summary>
public class AdminDto
{
    /// <summary>
    /// The unique identifier (GUID) of the administrator.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The role of the user (always "Administrator").
    /// </summary>
    public string Role { get; } = "Administrator";

    /// <summary>
    /// The administrator's email address.
    /// </summary>
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The administrator's contact email address.
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// The administrator's contact phone number.
    /// </summary>
    [StringLength(20)]
    [Phone]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// The date and time the administrator was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates whether the administrator is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// The administrator's first name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The administrator's last name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The administrator's patronymic.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string? Patronymic { get; set; }
}
