using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

public class CreateInvitationDto
{
    public Guid ReceiverId { get; set; }

    public Guid? VacancyId { get; set; }

    public Guid? ResumeId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [RegularExpression("EmployerToStudent|StudentToEmployer", ErrorMessage = "{0} must be 'EmployerToStudent' or 'StudentToEmployer'")]
    public string Type { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Message { get; set; }
}
