using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using StudHunter.API.ModelsDto.StudyPlanDto;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseStudyPlanService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Maps a StudyPlan entity to a StudyPlanDto.
    /// </summary>
    /// <param name="studyPlan">The study plan entity to map.</param>
    /// <returns>A StudyPlanDto representing the study plan.</returns>
    protected StudyPlanDto MapToStudyPlanDto(StudyPlan studyPlan)
    {
        return new StudyPlanDto
        {
            Id = studyPlan.Id,
            StudentId = studyPlan.StudentId,
            CourseNumber = studyPlan.CourseNumber,
            FacultyId = studyPlan.FacultyId,
            SpecialityId = studyPlan.SpecialityId,
            StudyForm = studyPlan.StudyForm,
            CourseIds = studyPlan.StudyPlanCourses.Select(spc => spc.CourseId).ToList()
        };
    }
}
