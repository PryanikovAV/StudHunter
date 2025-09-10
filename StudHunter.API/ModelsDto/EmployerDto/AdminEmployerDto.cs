namespace StudHunter.API.ModelsDto.EmployerDto;

/// <summary>
/// Data transfer object for an employer (administrative functions).
/// </summary>
public class AdminEmployerDto : EmployerDto
{
    /// <summary>
    /// Indicates whether the employer is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
