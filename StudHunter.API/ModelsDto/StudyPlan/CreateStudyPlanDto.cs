using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.StudyPlan;

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
    [RegularExpression("FullTime|PartTime|Correspondence", ErrorMessage = "{0} must be 'FullTime', 'PartTime', or 'Correspondence'")]
    public string StudyForm { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} is required")]
    [Range(2000, 2025)]
    public DateOnly BeginYear { get; set; }

    public List<Guid> CourseIds { get; set; } = new List<Guid>();
}
