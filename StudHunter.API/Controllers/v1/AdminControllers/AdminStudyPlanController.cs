using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.StudyPlanDto;
using StudHunter.API.Services;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres.Models;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Administrator))]
public class AdminStudyPlanController(AdminStudyPlanService adminStudyPlanService) : BaseController
{
    private readonly AdminStudyPlanService _adminStudyPlanService = adminStudyPlanService;

    /// <summary>
    /// Retrieves all study plans for admin moderation.
    /// </summary>
    /// <returns>A list of all study plans.</returns>
    /// <response code="200">Study plans retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<StudyPlanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStudyPlans()
    {
        var (studyPlans, statusCode, errorMessage) = await _adminStudyPlanService.GetAllStudyPlansAsync();
        return HandleResponse(studyPlans, statusCode, errorMessage);
    }

    /// <summary>
    /// Searches study plans by filters for employers.
    /// </summary>
    /// <param name="courseNumber">The course number filter (optional).</param>
    /// <param name="facultyId">The faculty ID filter (optional).</param>
    /// <param name="specialityId">The speciality ID filter (optional).</param>
    /// <param name="courseIds">The list of course IDs filter (optional).</param>
    /// <returns>A list of study plans matching the filters.</returns>
    /// <response code="200">Study plans retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<StudyPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchStudyPlans(
        [FromQuery] int? courseNumber,
        [FromQuery] Guid? facultyId,
        [FromQuery] Guid? specialityId,
        [FromQuery] List<Guid>? courseIds)
    {
        var (studyPlans, statusCode, errorMessage) = await _adminStudyPlanService.SearchStudyPlansAsync(courseNumber, facultyId, specialityId, courseIds);
        return HandleResponse(studyPlans, statusCode, errorMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<IActionResult> UpdateStudyPlan(Guid studentId, [FromBody] UpdateStudyPlanDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<bool>(false, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _adminStudyPlanService.UpdateStudyPlanAsync(studentId, dto);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
