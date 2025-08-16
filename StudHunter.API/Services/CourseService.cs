using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing courses.
/// </summary>
public class CourseService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    /// <returns>A tuple containing a list of all courses, an optional status code, and an optional error message.</returns>
    public async Task<(List<CourseDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllCoursesAsync()
    {
        var courses = await _context.Courses
        .Select(c => new CourseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        })
        .ToListAsync();

        return (courses, null, null);
    }

    /// <summary>
    /// Retrieves a course by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the course.</param>
    /// <returns>A tuple containing the course, an optional status code, and an optional error message.</returns>
    public async Task<(CourseDto? Entity, int? StatusCode, string? ErrorMessage)> GetCourseAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Course)));

        return (new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        }, null, null);
    }
}
