using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Resume;

public class CreateResumeDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2500)]
    public string? Description { get; set; }
}
