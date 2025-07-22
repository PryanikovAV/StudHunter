using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminResumeController(AdminResumeService adminResumeService) : ControllerBase
{
    private readonly AdminResumeService _adminResumeService = adminResumeService;

    [HttpGet]
    public async Task<IActionResult> GetAllResumes()
    {
        var resumes = await _adminResumeService.GetAllResumesAsync();
        return Ok(resumes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetResume(Guid id)
    {
        var resume = await _adminResumeService.GetResumeAsync(id);
        if (resume == null)
            return NotFound();
        return Ok(resume);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] UpdateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminResumeService.UpdateResumeAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResume(Guid id)
    {
        var (success, error) = await _adminResumeService.DeleteResumeAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
