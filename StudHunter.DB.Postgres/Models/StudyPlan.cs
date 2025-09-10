namespace StudHunter.DB.Postgres.Models;

public class StudyPlan
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public int CourseNumber { get; set; }

    public Guid FacultyId { get; set; }

    public Guid SpecialityId { get; set; }

    public StudyPlanForm StudyForm { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual Speciality Speciality { get; set; } = null!;

    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();

    public enum StudyPlanForm
    {
        FullTime,
        PartTime,
        Correspondence
    }
}
