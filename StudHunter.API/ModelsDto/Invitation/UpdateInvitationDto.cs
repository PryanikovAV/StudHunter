using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

public class UpdateInvitationDto
{
    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("Sent|Accepted|Rejected", ErrorMessage = "{0} must be 'Sent', 'Accepted' or 'Rejected'")]
    public string Status { get; set; } = string.Empty;
}
