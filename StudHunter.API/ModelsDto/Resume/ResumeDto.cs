using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Resume;

public class ResumeDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
