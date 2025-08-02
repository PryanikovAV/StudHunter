using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Student;

/// <summary>
/// Data transfer object for updating a student.
/// </summary>
public class UpdateStudentDto : BaseUpdateStudentDto
{
    /// <summary>
    /// The student's password.
    /// </summary>
    [StringLength(255, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters")]
    public string? Password { get; set; }
}
