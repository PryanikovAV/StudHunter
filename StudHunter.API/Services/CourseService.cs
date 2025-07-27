using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public class CourseService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<(List<CourseDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllCoursesAsync()
    {
        var courses = await _context.Courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToListAsync();

        return (courses, null, null);
    }

    public async Task<(CourseDto? Entity, int? StatusCode, string? ErrorMessage)> GetCourseAsync(Guid id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        #region Serializers
        if (course == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Course"));
        #endregion

        return (new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        }, null, null);
    }
}
