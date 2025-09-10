using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.CourseDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing courses with administrative privileges.
/// </summary>
public class AdminCourseService(StudHunterDbContext context) : BaseCourseService(context)
{
    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="dto">The data transfer object containing course details.</param>
    /// <returns>A tuple containing the created course, an optional status code, and an optional error message.</returns>
    public async Task<(CourseDto? Entity, int? StatusCode, string? ErrorMessage)> CreateCourseAsync(CreateCourseDto dto)
    {
        if (await _context.Courses.AnyAsync(c => c.Name == dto.Name))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Course), nameof(Course.Name)));

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

        return (MapToCourseDto(course), null, null);
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <param name="dto">The data transfer object containing updated course details.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateCourseAsync(Guid courseId, UpdateCourseDto dto)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Course)));
        if (dto.Name != null && await _context.Courses.AnyAsync(c => c.Name == dto.Name && c.Id != courseId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Course), nameof(Course.Name)));

        if (dto.Name != null)
            course.Name = dto.Name;
        if (dto.Description != null)
            course.Description = dto.Description;

        return await SaveChangesAsync<Course>();
    }

    /// <summary>
    /// Deletes a course.
    /// </summary>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteCourseAsync(Guid courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Course)));
        if (await _context.VacancyCourses.AnyAsync(vc => vc.CourseId == course.Id))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.CannotDelete(nameof(Course), nameof(Vacancy)));
        if (await _context.StudyPlanCourses.AnyAsync(spc => spc.CourseId == course.Id))
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.CannotDelete(nameof(Course), nameof(StudyPlan)));

        _context.Courses.Remove(course);

        return await SaveChangesAsync<Course>();
    }
}
