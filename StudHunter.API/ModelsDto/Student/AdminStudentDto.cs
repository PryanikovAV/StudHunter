namespace StudHunter.API.ModelsDto.Student;

/// <summary>
/// Data transfer object for a student (administrative functions).
/// </summary>
public class AdminStudentDto : StudentDto
{
    /// <summary>
    /// Indicates whether the student is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
