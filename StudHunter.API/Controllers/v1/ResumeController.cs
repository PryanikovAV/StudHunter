using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.BaseModelsDto;
using StudHunter.API.ModelsDto.ResumeDto;
using StudHunter.API.Services;
using StudHunter.DB.Postgres.Models;
using System.Security.Claims;

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
    /// Retrieves a resume by student ID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>The resume.</returns>
    /// <response code="200">Resume retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Resume not found.</response>
    /// <response code="410">Resume has been deleted.</response>
    [HttpGet("{studentId}")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResume(Guid studentId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<ResumeDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (resume, statusCode, errorMessage) = await _resumeService.GetResumeAsync(studentId, authUserId);
        return HandleResponse(resume, statusCode, errorMessage);
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
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> CreateResume([FromBody] CreateResumeDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<ResumeDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<ResumeDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (resume, statusCode, errorMessage) = await _resumeService.CreateResumeAsync(authUserId, dto);
        return HandleResponse(resume, statusCode, errorMessage, nameof(GetResume), new { studentId = resume?.StudentId });
    }

    /// <summary>
    /// Updates an existing resume.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated resume details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Resume updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Resume not found.</response>
    /// <response code="410">Resume has been deleted.</response>
    [HttpPut("{resumeId}")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateResume(Guid studentId, [FromBody] UpdateResumeDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<ResumeDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _resumeService.UpdateResumeAsync(authUserId, studentId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{studentId}/status")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> UpdateResumeStatus(Guid studentId, [FromBody] UpdateStatusDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<ResumeDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _resumeService.UpdateResumeStatusAsync(authUserId, studentId, dto);
        return HandleResponse(success, statusCode, errorMessage, nameof(Resume));
    }
}
