using System.ComponentModel.DataAnnotations;
namespace StudHunter.DB.Postgres.Models;

public class StudyPlan
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    [Range(1, 10)]
    public int CourseNumber { get; set; }

    public Guid FacultyId { get; set; }

    public Guid SpecialityId { get; set; }

    public StudyForms StudyForm { get; set; }

    public DateOnly BeginYear { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual Faculty Faculty { get; set; } = null!;
    public virtual Speciality Speciality { get; set; } = null!;
    public virtual ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();

    public enum StudyForms
    {
        [Display(Name = "Очная")]
        fulltime,
        [Display(Name = "Очно-заочная")]
        parttime,
        [Display(Name = "Заочная")]
        correspondence
    }
}
