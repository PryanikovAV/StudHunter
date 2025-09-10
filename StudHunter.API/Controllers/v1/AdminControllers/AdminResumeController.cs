using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.ResumeDto;
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
    [ProducesResponseType(typeof(List<ResumeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllResumes()
    {
        var (resumes, statusCode, errorMessage) = await _adminResumeService.GetAllResumesAsync();
        return HandleResponse(resumes, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a resume by studentId.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>The resume.</returns>
    /// <response code="200">Resume retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Resume not found.</response>
    /// <response code="410">Resume has been deleted.</response>
    [HttpGet("{studentId}")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResume(Guid studentId)
    {
        var (resume, statusCode, errorMessage) = await _adminResumeService.GetResumeAsync(studentId);
        return HandleResponse(resume, statusCode, errorMessage);
    }

    /// <summary>
    /// Updates an existing resume by studentId.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated resume details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Resume updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Resume not found.</response>
    [HttpPut("{studentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateResume(Guid studentId, [FromBody] UpdateResumeDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminResumeService.UpdateResumeAsync(studentId, dto);
        return HandleResponse(success, statusCode, errorMessage, nameof(Resume));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{studentId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateResumeStatus(Guid studentId, [FromBody] UpdateStatusDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        var (success, statusCode, errorMessage) = await _adminResumeService.UpdateResumeStatusAsync(studentId, dto);
        return HandleResponse(success, statusCode, errorMessage, nameof(Resume));
    }

    /// <summary>
    /// Deletes a resume (hard delete) by studentId.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Resume deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Resume not found.</response>
    [HttpDelete("{studentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteResume(Guid studentId)
    {
        var (success, statusCode, errorMessage) = await _adminResumeService.DeleteResumeAsync(studentId);
        return HandleResponse(success, statusCode, errorMessage, nameof(Resume));
    }
}
