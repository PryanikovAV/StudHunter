using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

/// <summary>
/// Data transfer object for updating an invitation.
/// </summary>
public class UpdateInvitationDto
{
    /// <summary>
    /// The status of the invitation (Sent, Accepted, or Rejected).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Sent|Accepted|Rejected", ErrorMessage = "{0} must be 'Sent', 'Accepted' or 'Rejected'")]
    public string Status { get; set; } = string.Empty;
}
