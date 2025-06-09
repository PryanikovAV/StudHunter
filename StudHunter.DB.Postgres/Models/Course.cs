using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class Course
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public virtual ICollection<VacancyCourse> VacancyCourses { get; set; } = new List<VacancyCourse>();
    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();
}
