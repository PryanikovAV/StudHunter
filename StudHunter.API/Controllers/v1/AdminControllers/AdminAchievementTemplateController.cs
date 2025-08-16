using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminAchievementTemplateController(AdminAchievementTemplateService administratorAchievementTemplateService) : BaseController
{
    private readonly AdminAchievementTemplateService _administratorAchievementTemplateService = administratorAchievementTemplateService;

    /// <summary>
    /// Retrieves all achievement templates.
    /// </summary>
    /// <returns>A list of achievement templates.</returns>
    /// <response code="200">Achievement templates retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AchievementTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllAchievementTemplates()
    {
        var (templates, statusCode, errorMessage) = await _administratorAchievementTemplateService.GetAllAchievementTemplatesAsync();
        return CreateAPIError(templates, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves an achievement template by its order number.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>The achievement template.</returns>
    /// <response code="200">Achievement template retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Achievement template not found.</response>
    [HttpGet("{orderNumber}")]
    [ProducesResponseType(typeof(AchievementTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAchievementTemplate(Guid id)
    {
        var (template, statusCode, errorMessage) = await _administratorAchievementTemplateService.GetAchievementTemplateAsync(id);
        return CreateAPIError(template, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new achievement template.
    /// </summary>
    /// <param name="dto">The data transfer object containing achievement template details.</param>
    /// <returns>The created achievement template.</returns>
    /// <response code="201">Achievement template created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="409">An achievement template with the specified Name or OrderNumber already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AchievementTemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAchievementTemplate([FromBody] CreateAchievementTemplateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (template, statusCode, errorMessage) = await _administratorAchievementTemplateService.CreateAchievementTemplateAsync(dto);
        return CreateAPIError(template, statusCode, errorMessage, nameof(GetAchievementTemplate), new { id = template?.Id });
    }

    /// <summary>
    /// Updates an existing achievement template.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the achievement template.</param>
    /// <param name="dto">The data transfer object containing updated achievement template details.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Achievement template updated successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Achievement template not found.</response>
    /// <response code="409">An achievement template with the specified Name or OrderNumber already exists.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAchievementTemplate(Guid id, [FromBody] UpdateAchievementTemplateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (success, statusCode, errorMessage) = await _administratorAchievementTemplateService.UpdateAchievementTemplateAsync(id, dto);
        return CreateAPIError<AchievementTemplateDto>(success, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes an achievement template.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the achievement template.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Achievement template deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Achievement template not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAchievementTemplate(Guid id)
    {
        var (success, statusCode, errorMessage) = await _administratorAchievementTemplateService.DeleteAchievementTemplateAsync(id);
        return CreateAPIError<AchievementTemplateDto>(success, statusCode, errorMessage);
    }
}
