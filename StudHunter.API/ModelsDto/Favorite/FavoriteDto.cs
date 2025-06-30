using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Favorite;

public class FavoriteDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public Guid? VacancyId { get; set; }

    public Guid? ResumeId { get; set; }

    public DateTime AddedAt { get; set; }
}
