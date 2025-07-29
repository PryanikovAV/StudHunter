using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class ResumeController(ResumeService resumeService) : BaseController
{
    private readonly ResumeService _resumeService = resumeService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResume()
    {
        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (resume, statusCode, errorMessage) = await _resumeService.GetResumeAsync(userId);
        return this.CreateAPIError(resume, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateResume([FromBody] CreateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var id = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (resume, statusCode, errorMessage) = await _resumeService.CreateResumeAsync(id, dto);
        return this.CreateAPIError(resume, statusCode, errorMessage, nameof(GetResume), new { id = resume?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResume([FromBody] UpdateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var id = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (success, statusCode, errorMessage) = await _resumeService.UpdateResumeAsync(id, dto);
        return this.CreateAPIError<ResumeDto>(success, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResume()
    {
        var id = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (success, statusCode, errorMessage) = await _resumeService.DeleteResumeAsync(id);
        return this.CreateAPIError<ResumeDto>(success, statusCode, errorMessage);
    }
}
