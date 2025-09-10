using System.ComponentModel.DataAnnotations;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto.StudyPlanDto;

public class CreateStudyPlanDto
{
    [Required(ErrorMessage = "{0} is required")]
    public Guid StudentId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Range(1, 7, ErrorMessage = "{0} must be between {1} and {2}")]
    public int CourseNumber { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public Guid FacultyId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public Guid SpecialityId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public StudyPlan.StudyPlanForm StudyForm { get; set; }

    public List<Guid> CourseIds { get; set; } = new List<Guid>();
}
