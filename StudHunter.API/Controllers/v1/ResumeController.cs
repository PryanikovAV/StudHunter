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
    public async Task<IActionResult> GetResumes()
    {
        var resumes = await _resumeService.GetResumesAsync();

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

    [HttpPost]
    public async Task<IActionResult> CreateResume([FromBody] CreateResumeDto dto)
    {
        var (resume, error) = await _resumeService.CreateResumeAsync(dto);

        if (error != null)
            return Conflict(new { error });

        return CreatedAtAction(nameof(GetResume), new { id = resume!.Id }, resume);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] UpdateResumeDto dto)
    {
        var (success, error) = await _resumeService.UpdateResumeAsync(id, dto);

        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResume(Guid id)
    {
        var (success, error) = await _resumeService.DeleteResumeAsync(id);
        
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }
}
