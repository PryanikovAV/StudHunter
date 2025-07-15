using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Resume;

public class UpdateResumeDto
{
    [MaxLength(255)]
    public string? Title { get; set; }

    [MaxLength(2500)]
    public string? Description { get; set; }
}
