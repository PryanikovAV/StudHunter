using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.CourseDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseCourseService(StudHunterDbContext context) : BaseService(context)
{
    protected static CourseDto MapToCourseDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
        };
    }

    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    /// <returns>A tuple containing a list of all courses, an optional status code, and an optional error message.</returns>
    public async Task<(List<CourseDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllCoursesAsync()
    {
        var courses = await _context.Courses
            .Select(c => MapToCourseDto(c))
            .OrderByDescending(c => c.Name)
            .ToListAsync();

        return (courses, null, null);
    }

    /// <summary>
    /// Retrieves a course by its ID.
    /// </summary>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>A tuple containing the course, an optional status code, and an optional error message.</returns>
    public async Task<(CourseDto? Entity, int? StatusCode, string? ErrorMessage)> GetCourseAsync(Guid courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Course)));

        return (MapToCourseDto(course), null, null);
    }

    /// <summary>
    /// Retrieves a course by its name.
    /// </summary>
    /// <param name="courseName"></param>
    /// <returns></returns>
    public async Task<(CourseDto? Entity, int? StatusCode, string? ErrorMessage)> GetCourseAsync(string courseName)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Name == courseName);
        if (course == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Course)));

        return (MapToCourseDto(course), null, null);
    }
}
