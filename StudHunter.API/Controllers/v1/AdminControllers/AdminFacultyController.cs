using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminFacultyController(AdminFacultyService adminFacultyService) : BaseController
{
    private readonly AdminFacultyService _adminFacultyService = adminFacultyService;

    [HttpGet]
    public async Task<IActionResult> GetAllFaculties()
    {
        var (faculties, statusCode, errorMessage) = await _adminFacultyService.GetAllFacultiesAsync();
        return this.CreateAPIError(faculties, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var (faculty, statusCode, errorMessage) = await _adminFacultyService.GetFacultyAsync(id);
        return this.CreateAPIError(faculty, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (faculty, statusCode, errorMessage) = await _adminFacultyService.CreateFacultyAsync(dto);
        return this.CreateAPIError(faculty, statusCode, errorMessage, nameof(GetFaculty), new { id = faculty?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFaculty(Guid id, [FromBody] UpdateFacultyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (success, statusCode, errorMessage) = await _adminFacultyService.UpdateFacultyAsync(id, dto);
        return this.CreateAPIError<FacultyDto>(success, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFaculty(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminFacultyService.DeleteFacultyAsync(id);
        return this.CreateAPIError<FacultyDto>(success, statusCode, errorMessage);
    }
}
