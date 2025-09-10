using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.InvitationDto;

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
}
