using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdministratorResumeController(AdministratorResumeService administratorResumeService) : ControllerBase
{
    private readonly AdministratorResumeService _administratorResumeService = administratorResumeService;

    [HttpGet]
    public async Task<IActionResult> GetAllResumes()
    {
        var resumes = await _administratorResumeService.GetAllResumesAsync();
        return Ok(resumes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetResume(Guid id)
    {
        var resume = await _administratorResumeService.GetResumeAsync(id);
        if (resume == null)
            return NotFound();
        return Ok(resume);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] UpdateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _administratorResumeService.UpdateResumeAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResume(Guid id)
    {
        var (success, error) = await _administratorResumeService.DeleteResumeAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
