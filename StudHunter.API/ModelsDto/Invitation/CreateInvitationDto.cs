using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

public class CreateInvitationDto
{
    public Guid ReceiverId { get; set; }

    public Guid? VacancyId { get; set; }

    public Guid? ResumeId { get; set; }

    [Required, RegularExpression("EmployerToStudent|StudentToEmployer", ErrorMessage = "Type must be 'EmployerToStudent' or 'StudentToEmployer'")]
    public string Type { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Message { get; set; }
}
