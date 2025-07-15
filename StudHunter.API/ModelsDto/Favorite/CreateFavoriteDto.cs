namespace StudHunter.API.ModelsDto.Favorite;

public class CreateFavoriteDto
{
    public Guid? VacancyId { get; set; }

    public Guid? ResumeId { get; set; }
}
