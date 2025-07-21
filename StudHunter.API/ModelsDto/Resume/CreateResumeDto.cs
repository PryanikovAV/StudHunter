using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Resume;

public class CreateResumeDto
{
    [Required(ErrorMessage = "{0} is required")]
    public Guid StudentId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
