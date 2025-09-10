using System.ComponentModel.DataAnnotations;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto.StudyPlanDto;

public class StudyPlanDto
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    [Range(1, 7)]
    public int CourseNumber { get; set; }

    public Guid FacultyId { get; set; }

    public Guid SpecialityId { get; set; }

    public StudyPlan.StudyPlanForm StudyForm { get; set; }

    public List<Guid> CourseIds { get; set; } = new List<Guid>();
}
