using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Speciality
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public virtual ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
}
