using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

public class UpdateStudentDto : BaseUpdateStudentDto
{
    [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1}")]
    public string? Password { get; set; }
}
