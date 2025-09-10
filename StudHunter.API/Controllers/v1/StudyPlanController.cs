using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.StudyPlanDto;
using StudHunter.API.Services;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class StudyPlanController(StudyPlanService studyPlanService) : BaseController
{
    private readonly StudyPlanService _studyPlanService = studyPlanService;

    /// <summary>
    /// Retrieves a study plan by student ID for the authenticated user.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>The study plan.</returns>
    /// <response code="200">Study plan retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Study plan not found.</response>
    [HttpGet("student/{studyPlanId}")]
    [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudyPlan(Guid studentId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<StudyPlanDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (studyPlan, statusCode, errorMessage) = await _studyPlanService.GetStudyPlanAsync(authUserId, studentId);
        return HandleResponse(studyPlan, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new study plan for the authenticated student.
    /// </summary>
    /// <param name="dto">The data transfer object containing study plan details.</param>
    /// <returns>The created study plan.</returns>
    /// <response code="201">Study plan created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to create this study plan.</response>
    /// <response code="409">Study plan already exists for this student.</response>
    [HttpPost]
    [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudyPlan([FromBody] CreateStudyPlanDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<StudyPlanDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<StudyPlanDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (studyPlan, statusCode, errorMessage) = await _studyPlanService.CreateStudyPlanAsync(authUserId, dto);
        return HandleResponse(studyPlan, statusCode, errorMessage, nameof(GetStudyPlan), new { studyPlanId = studyPlan?.Id });
    }

    /// <summary>
    /// Updates a study plan for the authenticated student dy studentId.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="dto">The data transfer object containing updated study plan details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Study plan updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to update this study plan.</response>
    /// <response code="404">Study plan not found.</response>
    [HttpPut("{studyPlanId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudyPlan(Guid studentId, [FromBody] UpdateStudyPlanDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _studyPlanService.UpdateStudyPlanAsync(authUserId, studentId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
