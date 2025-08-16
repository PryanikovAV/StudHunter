using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing resumes.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class ResumeController(ResumeService resumeService) : BaseController
{
    private readonly ResumeService _resumeService = resumeService;

    /// <summary>
    /// Retrieves a resume by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>The resume.</returns>
    /// <response code="200">Resume retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Resume not found.</response>
    /// <response code="410">Resume has been deleted.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> GetResume(Guid id)
    {
        var (resume, statusCode, errorMessage) = await _resumeService.GetResumeAsync(id);
        return CreateAPIError(resume, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new resume.
    /// </summary>
    /// <param name="dto">The data transfer object containing resume details.</param>
    /// <returns>The created resume.</returns>
    /// <response code="201">Resume created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Student not found.</response>
    /// <response code="409">A resume for the student already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateResume([FromBody] CreateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var studentId = Guid.NewGuid(); // TODO: Replace Guid.NewGuid() with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (resume, statusCode, errorMessage) = await _resumeService.CreateResumeAsync(studentId, dto);
        return CreateAPIError(resume, statusCode, errorMessage, nameof(GetResume), new { id = resume?.Id });
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
    /// <response code="404">Resume not found.</response>
    /// <response code="410">Resume has been deleted.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateResume(Guid id, [FromBody] UpdateResumeDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (success, statusCode, errorMessage) = await _resumeService.UpdateResumeAsync(id, dto);
        return CreateAPIError<ResumeDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a resume (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the resume.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Resume deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Resume not found.</response>
    /// <response code="410">Resume has been deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteResume(Guid id)
    {
        var (success, statusCode, errorMessage) = await _resumeService.DeleteResumeAsync(id);
        return CreateAPIError<ResumeDto>(success, statusCode, errorMessage);
    }
}
