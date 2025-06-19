namespace StudHunter.DB.Postgres.Models;

public class StudyPlanCourse
{
    public Guid StudyPlanId { get; set; }

    public Guid CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;
    public virtual StudyPlan StudyPlan { get; set; } = null!;
}
