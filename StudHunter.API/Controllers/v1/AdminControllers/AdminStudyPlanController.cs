using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.StudyPlan;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminStudyPlanController(AdminStudyPlanService adminStudyPlanService) : BaseController
{
    private readonly AdminStudyPlanService _adminStudyPlanService = adminStudyPlanService;

    /// <summary>
    /// Retrieves all study plans.
    /// </summary>
    /// <returns>A list of all study plans.</returns>
    /// <response code="200">Study plans retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<StudyPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllStudyPlans()
    {
        var (studyPlans, statusCode, errorMessage) = await _adminStudyPlanService.GetAllStudyPlansAsync();
        return CreateAPIError(studyPlans, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a study plan by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the study plan.</param>
    /// <returns>The study plan.</returns>
    /// <response code="200">Study plan retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Study plan not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudyPlan(Guid id)
    {
        var (studyPlan, statusCode, errorMessage) = await _adminStudyPlanService.GetStudyPlanAsync(id);
        return CreateAPIError(studyPlan, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a study plan (hard or soft delete).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the study plan.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Study plan deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Study plan not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudyPlan(Guid id, [FromQuery] bool hardDelete = false)
    {
        var (success, statusCode, errorMessage) = await _adminStudyPlanService.DeleteStudyPlanAsync(id, hardDelete);
        return CreateAPIError<StudyPlanDto>(success, statusCode, errorMessage);
    }
}
