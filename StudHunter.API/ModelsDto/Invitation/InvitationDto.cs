using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

/// <summary>
/// Data transfer object for an invitation.
/// </summary>
public class InvitationDto
{
    /// <summary>
    /// The unique identifier (GUID) of the invitation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier (GUID) of the sender.
    /// </summary>
    public Guid SenderId { get; set; }

    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier (GUID) of the receiver.
    /// </summary>
    public Guid ReceiverId { get; set; }

    public string ReceiverEmail { get; set; } = string.Empty;

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
    [Required]
    [RegularExpression("EmployerToStudent|StudentToEmployer")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The status of the invitation (Sent, Accepted, or Rejected).
    /// </summary>
    [Required]
    [RegularExpression("Sent|Accepted|Rejected")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The date and time the invitation was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time the invitation was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The status of the associated vacancy.
    /// </summary>
    public string? VacancyStatus { get; set; }

    /// <summary>
    /// The status of the associated resume.
    /// </summary>
    public string? ResumeStatus { get; set; }
}
