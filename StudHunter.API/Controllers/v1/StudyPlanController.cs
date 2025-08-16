using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.StudyPlan;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class StudyPlanController(StudyPlanService studyPlanService) : BaseController
{
    private readonly StudyPlanService _studyPlanService = studyPlanService;

    /// <summary>
    /// Retrieves a study plan by student ID.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <returns>The study plan.</returns>
    /// <response code="200">Study plan retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Study plan not found.</response>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudyPlanByStudentId(Guid studentId)
    {
        var (studyPlan, statusCode, errorMessage) = await _studyPlanService.GetStudyPlanByStudentAsync(studentId);
        return CreateAPIError(studyPlan, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new study plan.
    /// </summary>
    /// <param name="dto">The data transfer object containing study plan details.</param>
    /// <returns>The created study plan.</returns>
    /// <response code="201">Study plan created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Student, faculty, or speciality not found.</response>
    /// <response code="409">Study plan for the student already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudyPlan([FromBody] CreateStudyPlanDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (studyPlan, statusCode, errorMessage) = await _studyPlanService.CreateStudyPlanAsync(dto);
        return CreateAPIError(studyPlan, statusCode, errorMessage, nameof(GetStudyPlanByStudentId), new { studentId = studyPlan?.StudentId });
    }

    /// <summary>
    /// Updates an existing study plan.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the study plan.</param>
    /// <param name="dto">The data transfer object containing updated study plan details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Study plan updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Study plan, faculty, or speciality not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudyPlan(Guid id, [FromBody] UpdateStudyPlanDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (success, statusCode, errorMessage) = await _studyPlanService.UpdateStudyPlanAsync(id, dto);
        return CreateAPIError<StudyPlanDto>(success, statusCode, errorMessage);
    }
}
