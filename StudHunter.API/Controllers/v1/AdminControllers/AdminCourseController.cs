using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminCourseController(AdminCourseService adminCourseService) : BaseController
{
    private readonly AdminCourseService _adminCourseService = adminCourseService;

    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        var (courses, statusCode, errorMessage) = await _adminCourseService.GetAllCoursesAsync();
        return this.CreateAPIError(courses, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var (course, statusCode, errorMessage) = await _adminCourseService.GetCourseAsync(id);
        return this.CreateAPIError(course, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (course, statusCode, errorMessage) = await _adminCourseService.CreateCourseAsync(dto);
        return this.CreateAPIError(course, statusCode, errorMessage, nameof(GetCourse), new { id = course?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (course, statusCode, errorMessage) = await _adminCourseService.UpdateCourseAsync(id, dto);
        return this.CreateAPIError<CourseDto>(course, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var (course, statusCode, errorMessage) = await _adminCourseService.DeleteCourseAsync(id);
        return this.CreateAPIError<CourseDto>(course, statusCode, errorMessage);
    }
}
