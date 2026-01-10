namespace StudHunter.DB.Postgres.Models;

public class StudyPlan
{
    public Guid Id { get; init; }
    public Guid StudentId { get; set; }
    public int CourseNumber { get; set; }
    public Guid? FacultyId { get; set; }  // TODO: not nullable после тестов
    public Guid? SpecialityId { get; set; }  // TODO: not nullable после тестов
    public StudyPlanForm StudyForm { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public virtual Student Student { get; set; } = null!;
    public virtual Faculty Faculty { get; set; } = null!;
    public virtual Speciality Speciality { get; set; } = null!;
    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new HashSet<StudyPlanCourse>();
    public enum StudyPlanForm
    {
        FullTime = 0,
        PartTime = 1,
        Correspondence = 2
    }
}
