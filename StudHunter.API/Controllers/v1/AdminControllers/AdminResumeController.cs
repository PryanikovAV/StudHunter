using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminResumeController(AdminResumeService adminResumeService) : BaseController
{
    private readonly AdminResumeService _adminResumeService = adminResumeService;

    [HttpGet]
    public async Task<IActionResult> GetAllResumes()
    {
        var (resumes, statusCode, errorMessage) = await _adminResumeService.GetAllResumesAsync();
        return this.CreateAPIError(resumes, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResume(Guid id)
    {
        var (resume, statusCode, errorMessage) = await _adminResumeService.GetResumeAsync(id);
        return this.CreateAPIError(resume, statusCode, errorMessage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] AdminUpdateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (success, statusCode, errorMessage) = await _adminResumeService.UpdateResumeAsync(id, dto);
        return this.CreateAPIError<AdminResumeDto>(success, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResume(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminResumeService.DeleteResumeAsync(id);
        return this.CreateAPIError<AdminResumeDto>(success, statusCode, errorMessage);
    }
}
