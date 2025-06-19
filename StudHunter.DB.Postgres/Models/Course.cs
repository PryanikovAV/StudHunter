namespace StudHunter.DB.Postgres.Models;

public class Course
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<VacancyCourse> VacancyCourses { get; set; } = new List<VacancyCourse>();
    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();
}
