namespace StudHunter.DB.Postgres.Models;

public class StudyPlanCourse
{
    public Guid StudyPlanId { get; init; }
    public Guid CourseId { get; init; }

    public virtual Course Course { get; set; } = null!;
    public virtual StudyPlan StudyPlan { get; set; } = null!;
}
