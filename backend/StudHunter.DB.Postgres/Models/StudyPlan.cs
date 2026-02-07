namespace StudHunter.DB.Postgres.Models;

public class StudyPlan
{
    public Guid Id { get; init; }
    public Guid StudentId { get; set; }
    public Guid? UniversityId { get; set; }
    public Guid? FacultyId { get; set; }
    public Guid? StudyDirectionId { get; set; }
    public Guid? DepartmentId { get; set; }
    public int CourseNumber { get; set; }
    public StudyPlanForm StudyForm { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual University? University { get; set; }
    public virtual Faculty? Faculty { get; set; }
    public virtual StudyDirection? StudyDirection { get; set; }
    public virtual Department? Department { get; set; }
    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new HashSet<StudyPlanCourse>();
    public enum StudyPlanForm
    {
        FullTime = 0,
        PartTime = 1,
        Correspondence = 2
    }
}
