using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.Models;

public class StudyPlanDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [Required, Range(1, 10)]
    public int CourseNumber { get; set; }

    [Required]
    public Guid FacultyId { get; set; }

    public string FacultyName { get; set; } = string.Empty;

    [Required]
    public Guid SpecialityId { get; set; }

    public string SpecialityName { get; set; } = string.Empty;

    [Required]
    public string StudyForm { get; set; } = string.Empty;

    [Required]
    public DateOnly BeginYear { get; set; }

    public List<Guid> StudyPlanCourseIds { get; set; } = new List<Guid>();
}
