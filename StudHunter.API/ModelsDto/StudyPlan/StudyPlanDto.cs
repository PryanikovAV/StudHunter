using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.StudyPlan;

public class StudyPlanDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [Required]
    [Range(1, 7)]
    public int CourseNumber { get; set; }

    [Required]
    public Guid FacultyId { get; set; }

    [Required]
    public Guid SpecialityId { get; set; }

    [Required]
    [RegularExpression("FullTime|PartTime|Correspondence")]
    public string StudyForm { get; set; } = string.Empty;

    public List<Guid> CourseIds { get; set; } = new List<Guid>();
}
