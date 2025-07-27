namespace StudHunter.API.ModelsDto.Employer;

public class AdminUpdateEmployerDto : BaseUpdateEmployerDto
{   
    public bool? AccreditationStatus { get; set; }

    public bool? IsDeleted { get; set; }
}
