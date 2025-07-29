namespace StudHunter.API.ModelsDto.Resume;

public class AdminUpdateResumeDto : UpdateResumeDto
{
    public bool? IsDeleted { get; set; }
}
