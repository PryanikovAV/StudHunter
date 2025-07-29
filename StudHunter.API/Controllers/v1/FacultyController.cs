using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class FacultyController(FacultyService facultyService) : BaseController
{
    private readonly FacultyService _facultyService = facultyService;

    [HttpGet]
    public async Task<IActionResult> GetAllFaculties()
    {
        var (faculties, statusCode, errorMessage) = await _facultyService.GetAllFacultiesAsync();
        return this.CreateAPIError(faculties, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var (faculty, statusCode, errorMessage) = await _facultyService.GetFacultyAsync(id);
        return this.CreateAPIError(faculty, statusCode, errorMessage);
    }
}
