using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.StudentDto;

/// <summary>
/// Data transfer object for updating a student (administrative functions).
/// </summary>
public class AdminUpdateStudentDto : BaseUpdateStudentDto
{
    /// <summary>
    /// Indicates whether the student is deleted (optional).
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public bool? IsDeleted { get; set; }
}
