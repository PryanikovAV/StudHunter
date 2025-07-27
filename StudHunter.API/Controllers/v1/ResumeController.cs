using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class ResumeController(ResumeService resumeService) : ControllerBase
{
    private readonly ResumeService _resumeService = resumeService;

    [HttpGet]
    public async Task<IActionResult> GetAllResumes()
    {
        var resumes = await _resumeService.GetAllResumesAsync();
        return Ok(resumes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetResume(Guid id)
    {
        var resume = await _resumeService.GetResumeAsync(id);
        if (resume == null)
            return NotFound();
        return Ok(resume);
    }
    // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateResume([FromBody] CreateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var studentId = Guid.NewGuid();
        var (resume, error) = await _resumeService.CreateResumeAsync(studentId, dto);
        if (resume == null)
            return error == null ? NotFound() : BadRequest(new { error });
        return CreatedAtAction(nameof(GetResume), new { id = resume!.Id }, resume);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] UpdateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _resumeService.UpdateResumeAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> SoftDeleteResume(Guid id)
    {
        var (success, error) = await _resumeService.DeleteResumeAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
