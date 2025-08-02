using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

/// <summary>
/// Data transfer object for updating an employer.
/// </summary>
public class UpdateEmployerDto : BaseUpdateEmployerDto
{
    /// <summary>
    /// The employer's password.
    /// </summary>
    [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string? Password { get; set; }
}
