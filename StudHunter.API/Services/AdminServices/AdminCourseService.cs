using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Course;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing courses with administrative privileges.
/// </summary>
public class AdminCourseService(StudHunterDbContext context) : CourseService(context)
{
    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="dto">The data transfer object containing course details.</param>
    /// <returns>A tuple containing the created course, an optional status code, and an optional error message.</returns>
    public async Task<(CourseDto? Entity, int? StatusCode, string? ErrorMessage)> CreateCourseAsync(CreateCourseDto dto)
    {
        if (await _context.Courses.AnyAsync(c => c.Name == dto.Name))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Course), "name"));

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Courses.Add(course);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Course>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        }, null, null);
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the course.</param>
    /// <param name="dto">The data transfer object containing updated course details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateCourseAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Course)));

        if (dto.Name != null && await _context.Courses.AnyAsync(c => c.Name == dto.Name && c.Id != id))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Course), "name"));

        if (dto.Name != null)
            course.Name = dto.Name;
        if (dto.Description != null)
            course.Description = dto.Description;

        return await SaveChangesAsync<Course>();
    }

    /// <summary>
    /// Deletes a course.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the course.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteCourseAsync(Guid id)
    {
        if (await _context.VacancyCourses.AnyAsync(vc => vc.CourseId == id))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.CannotDelete(nameof(Course), nameof(Vacancy)));

        if (await _context.StudyPlanCourses.AnyAsync(spc => spc.CourseId == id))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.CannotDelete(nameof(Course), nameof(StudyPlan)));

        return await DeleteEntityAsync<Course>(id, hardDelete: true);
    }
}
