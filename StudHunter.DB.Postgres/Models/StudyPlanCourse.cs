using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class StudyPlanCourse
{
    [Required]
    public Guid StudyPlanId { get; set; }

    [Required]
    public Guid CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;
    public virtual StudyPlan StudyPlan { get; set; } = null!;
}
