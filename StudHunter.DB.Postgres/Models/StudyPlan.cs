using StudHunter.DB.Postgres.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class StudyPlan : IEntity
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public int CourseNumber { get; set; } = 1;

    public Guid FacultyId { get; set; } = Guid.Empty;

    public Guid SpecialityId { get; set; } = Guid.Empty;

    public StudyForms StudyForm { get; set; } = StudyForms.fulltime;

    public DateOnly BeginYear { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public virtual Student Student { get; set; } = null!;
    public virtual Faculty Faculty { get; set; } = null!;
    public virtual Speciality Speciality { get; set; } = null!;
    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();

    public enum StudyForms
    {
        fulltime,
        parttime,
        correspondence
    }
}
