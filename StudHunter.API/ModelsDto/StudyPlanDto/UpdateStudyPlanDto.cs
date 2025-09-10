using System.ComponentModel.DataAnnotations;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto.StudyPlanDto;

public class UpdateStudyPlanDto
{
    [Range(1, 7, ErrorMessage = "{0} must be between {1} and {2}")]
    public int? CourseNumber { get; set; }

    public Guid? FacultyId { get; set; }

    public Guid? SpecialityId { get; set; }

    public StudyPlan.StudyPlanForm? StudyForm { get; set; }

    public List<Guid>? CourseIds { get; set; }
}
