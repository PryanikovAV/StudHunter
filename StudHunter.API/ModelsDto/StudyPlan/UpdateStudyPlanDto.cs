using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.StudyPlan;

public class UpdateStudyPlanDto
{
    [Range(1, 7, ErrorMessage = "{0} must be between {1} and {2}")]
    public int? CourseNumber { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? SpecialityId { get; set; }

    [RegularExpression("FullTime|PartTime|Correspondence", ErrorMessage = "{0} must be 'FullTime', 'PartTime' or 'Correspondence'")]
    public string? StudyForm {  get; set; }

    public List<Guid>? CourseIds { get; set; }
}
