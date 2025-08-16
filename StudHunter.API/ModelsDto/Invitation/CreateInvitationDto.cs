using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

/// <summary>
/// Data transfer object for creating an invitation.
/// </summary>
public class CreateInvitationDto
{
    /// <summary>
    /// The unique identifier (GUID) of the receiver.
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the vacancy.
    /// </summary>
    public Guid? VacancyId { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the resume.
    /// </summary>
    public Guid? ResumeId { get; set; }

    /// <summary>
    /// The type of the invitation (EmployerToStudent or StudentToEmployer).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("EmployerToStudent|StudentToEmployer", ErrorMessage = "{0} must be 'EmployerToStudent' or 'StudentToEmployer'")]
    public string Type { get; set; } = string.Empty;
}
