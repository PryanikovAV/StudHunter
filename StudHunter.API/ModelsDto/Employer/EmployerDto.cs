using StudHunter.API.ModelsDto.UserAchievement;
using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Employer;

public class EmployerDto
{
    public Guid Id { get; set; }

    public string Role => "Employer";

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [EmailAddress, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Phone, MaxLength(20)]
    public string? ContactPhone { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public bool AccreditationStatus { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Url, MaxLength(255)]
    public string? Website { get; set; }

    [MaxLength(255)]
    public string? Specialization { get; set; }

    public List<Guid> VacancyIds { get; set; } = new List<Guid>();

    public List<UserAchievementDto> Achievements { get; set; } = new List<UserAchievementDto>();
}
