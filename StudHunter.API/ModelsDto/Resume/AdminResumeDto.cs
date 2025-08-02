namespace StudHunter.API.ModelsDto.Resume;

/// <summary>
/// Data transfer object for a resume (administrative functions).
/// </summary>
public class AdminResumeDto : ResumeDto
{
    /// <summary>
    /// Indicates whether the resume is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
