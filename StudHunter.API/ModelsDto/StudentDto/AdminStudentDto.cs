using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.StudentDto;

/// <summary>
/// Data transfer object representing a student for administrative purposes.
/// </summary>
public class AdminStudentDto : StudentDto
{
    /// <summary>
    /// Indicates whether the student is deleted.
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; }
}
