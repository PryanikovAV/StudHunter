using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing resumes with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminResumeController(AdminResumeService adminResumeService) : BaseController
{
    private readonly AdminResumeService _adminResumeService = adminResumeService;

    /// <summary>
    /// Retrieves all resumes.
    /// </summary>
    /// <returns>A list of all resumes.</returns>
    /// <response code="200">Resumes retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AdminResumeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllResumes()
    {
        var (resumes, statusCode, errorMessage) = await _adminResumeService.GetAllResumesAsync();
        return HandleResponse(resumes, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a resume by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>The resume.</returns>
    /// <response code="200">Resume retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Resume not found.</response>
    /// <response code="410">Resume has been deleted.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdminResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResume(Guid id)
    {
        var (resume, statusCode, errorMessage) = await _adminResumeService.GetResumeAsync(id);
        return HandleResponse(resume, statusCode, errorMessage);
    }

    /// <summary>
    /// Updates an existing resume.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <param name="dto">The data transfer object containing updated resume details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Resume updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Resume not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] AdminUpdateResumeDto dto)
    {
        var validationResult = ValidateModel();
        if (!validationResult.IsSuccess())
            return validationResult;

        var (success, statusCode, errorMessage) = await _adminResumeService.UpdateResumeAsync(id, dto);
        return HandleResponse(success, statusCode, errorMessage, nameof(Resume));
    }

    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateResumeStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        var validationResult = ValidateModel();
        if (!validationResult.IsSuccess())
                return validationResult;
        var (success, statusCode, errorMessage) = await _adminResumeService.UpdateResumeStatusAsync(id, dto.IsDeleted);
        return HandleResponse(success, statusCode, errorMessage, nameof(Resume));
    }

    /// <summary>
    /// Deletes a resume (hard delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Resume deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Resume not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteResume(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminResumeService.DeleteResumeAsync(id);
        return HandleResponse(success, statusCode, errorMessage, nameof(Resume));
    }
}
