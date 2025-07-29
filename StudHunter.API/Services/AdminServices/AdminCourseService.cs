using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Course;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminCourseService(StudHunterDbContext context) : CourseService(context)
{
    public async Task<(CourseDto? Entity, int? StatusCode, string? ErrorMessage)> CreateCourseAsync(CreateCourseDto dto)
    {
        #region Serializers
        var nameExists = await _context.Courses.AnyAsync(c => c.Name == dto.Name);
        if (nameExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Course", "Name"));
        #endregion

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

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateCourseAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        #region Serializers
        if (course == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Course"));

        var nameExists = await _context.Courses.AnyAsync(c => c.Name == dto.Name && c.Id != id);
        if (nameExists)
            return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Course", "Name"));
        #endregion

        if (dto.Name != null)
            course.Name = dto.Name;
        if (dto.Description != null)
            course.Description = dto.Description;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Course>();

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteCourseAsync(Guid id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        #region Serializers
        var courseAssociatedVacancy = await _context.VacancyCourses.AnyAsync(vc => vc.CourseId == id);
        if (courseAssociatedVacancy)
            return (false, StatusCodes.Status400BadRequest, "Cannot delete course associated with vacancies");

        var courseAssociatedStudyPlan = await _context.StudyPlanCourses.AnyAsync(spc => spc.CourseId == id);
        if (courseAssociatedStudyPlan)
            return (false, StatusCodes.Status400BadRequest, "Cannot delete course associated with study plans");
        #endregion

        return await DeleteEntityAsync<Course>(id, hardDelete: true);
    }
}
