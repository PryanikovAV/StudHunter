namespace StudHunter.API.ModelsDto.Vacancy;

public class AdminUpdateVacancyDto : UpdateVacancyDto
{
    public bool? IsDeleted { get; set; }
}
