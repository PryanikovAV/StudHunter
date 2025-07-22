using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminCourseService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<CourseDto?> GetCourseAsync(Guid id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return null;

        return new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        };
    }

    public async Task<(CourseDto? Course, string? Error)> CreateCourseAsync(CreateCourseDto dto)
    {
        if (await _context.Courses
        .AnyAsync(c => c.Name == dto.Name))
            return (null, "Course with this name already exists.");

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Courses.Add(course);

        var (success, error) = await SaveChangesAsync("create", "Course");
        if (!success)
            return (null, error);

        return (new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        }, null);
    }

    public async Task<(bool Success, string? Error)> UpdateCourseAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return (false, "Course not found.");

        if (await _context.Courses.AnyAsync(c => c.Name == dto.Name && c.Id != id))
            return (false, "Course with this name already exists.");

        if (dto.Name != null)
            course.Name = dto.Name;
        if (dto.Description != null)
            course.Description = dto.Description;

        return await SaveChangesAsync("update", "Course");
    }

    public async Task<(bool Success, string? Error)> DeleteCourseAsync(Guid id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return (false, "Course not found");

        if (await _context.VacancyCourses.AnyAsync(vc => vc.CourseId == id))
            return (false, "Cannot delete course associated with vacancies");
        if (await _context.StudyPlanCourses.AnyAsync(spc => spc.CourseId == id))
            return (false, "Cannot delete course associated with study plans");

        return await HardDeleteEntityAsync<Course>(id);
    }
}
