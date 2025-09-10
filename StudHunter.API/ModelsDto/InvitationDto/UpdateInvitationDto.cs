using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.InvitationDto;

/// <summary>
/// Data transfer object for updating an invitation.
/// </summary>
public class UpdateInvitationDto
{
    /// <summary>
    /// The status of the invitation (Sent, Accepted, or Rejected).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Accepted|Rejected", ErrorMessage = "{0} must be 'Accepted' or 'Rejected'")]
    public string Status { get; set; } = string.Empty;
}
