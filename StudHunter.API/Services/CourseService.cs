using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public class CourseService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        return await _context.Courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        })
        .ToListAsync();
    }

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
}
