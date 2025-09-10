using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.EmployerDto;

/// <summary>
/// Base data transfer object for updating an employer.
/// </summary>
public class BaseUpdateEmployerDto
{
    /// <summary>
    /// The employer's email address.
    /// </summary>
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    public string? Email { get; set; }

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
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Name { get; set; }

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
}
