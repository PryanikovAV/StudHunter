using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Resume;

public class UpdateResumeDto
{
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Title { get; set; }

    [StringLength(2500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
