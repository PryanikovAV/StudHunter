using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

public class UpdateInvitationDto
{
    [Required, RegularExpression("Sent|Accepted|Rejected", ErrorMessage = "Status must be 'Sent', 'Accepted' or 'Rejected'")]
    public string Status { get; set; } = string.Empty;
}
