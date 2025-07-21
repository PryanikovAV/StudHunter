using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Invitation;

public class InvitationDto
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public Guid? VacancyId { get; set; }

    public Guid? ResumeId { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Message { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? VacancyStatus { get; set; }

    public string? ResumeStatus { get; set; }
}
