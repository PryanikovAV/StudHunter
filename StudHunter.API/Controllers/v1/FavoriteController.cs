using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.FavoriteDto;
using StudHunter.API.Services;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

/// <summary>
/// Controller for managing favorites.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class FavoriteController(FavoriteService favoriteService) : BaseController
{
    private readonly FavoriteService _favoriteService = favoriteService;

    /// <summary>
    /// Retrieves all favorites for the authenticated user.
    /// </summary>
    /// <returns>A list of favorites.</returns>
    /// <response code="200">Favorites retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllFavorites()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<FavoriteDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorites, statusCode, errorMessage) = await _favoriteService.GetAllFavoritesByUserAsync(authUserId);
        return HandleResponse(favorites, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a favorite by its ID for the authenticated user.
    /// </summary>
    /// <param name="favoriteId">The unique identifier (GUID) of the favorite.</param>
    /// <returns>The favorite.</returns>
    /// <response code="200">Favorite retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Favorite not found.</response>
    [HttpGet("{favoriteId}")]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFavorite(Guid favoriteId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<FavoriteDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorite, statusCode, errorMessage) = await _favoriteService.GetFavoriteAsync(authUserId, favoriteId);
        return HandleResponse(favorite, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves all favorite vacancies for the authenticated student.
    /// </summary>
    /// <returns>A list of favorite vacancies.</returns>
    /// <response code="200">Favorite vacancies retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a student.</response>
    [HttpGet("vacancies")]
    [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFavoriteVacancies()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<FavoriteDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorites, statusCode, errorMessage) = await _favoriteService.GetFavoriteVacanciesAsync(authUserId);
        return HandleResponse(favorites, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves all favorite employers for the authenticated student.
    /// </summary>
    /// <returns>A list of favorite employers.</returns>
    /// <response code="200">Favorite employers retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a student.</response>
    [HttpGet("employers")]
    [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFavoriteEmployers()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<FavoriteDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorites, statusCode, errorMessage) = await _favoriteService.GetFavoriteEmployersAsync(authUserId);
        return HandleResponse(favorites, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves all favorite students for the authenticated employer.
    /// </summary>
    /// <returns>A list of favorite students.</returns>
    /// <response code="200">Favorite students retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not an accredited employer.</response>
    [HttpGet("students")]
    [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFavoriteStudents()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<List<FavoriteDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorites, statusCode, errorMessage) = await _favoriteService.GetFavoriteStudentsAsync(authUserId);
        return HandleResponse(favorites, statusCode, errorMessage);
    }

    /// <summary>
    /// Checks if a vacancy, employer, or student is favorited by the authenticated user.
    /// </summary>
    /// <param name="vacancyId">The ID of the vacancy (optional).</param>
    /// <param name="employerId">The ID of the employer (optional).</param>
    /// <param name="studentId">The ID of the student (optional).</param>
    /// <returns>True if the entity is favorited, false otherwise.</returns>
    /// <response code="200">Check completed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("is-favorite")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IsFavorite([FromQuery] Guid? vacancyId, [FromQuery] Guid? employerId, [FromQuery] Guid? studentId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (exists, statusCode, errorMessage) = await _favoriteService.IsFavoriteAsync(authUserId, vacancyId, employerId, studentId);
        return HandleResponse(exists, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new favorite for the authenticated user.
    /// </summary>
    /// <param name="dto">The data transfer object containing favorite details.</param>
    /// <returns>The created favorite.</returns>
    /// <response code="201">Favorite created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to favorite this entity.</response>
    /// <response code="404">Target entity not found.</response>
    /// <response code="409">Favorite already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto dto)
    {
        if (!ValidateModel())
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return HandleResponse<FavoriteDto>(null, StatusCodes.Status400BadRequest, string.Join("; ", errors));
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<FavoriteDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorite, statusCode, errorMessage) = await _favoriteService.CreateFavoriteAsync(authUserId, dto);
        return HandleResponse(favorite, statusCode, errorMessage, nameof(GetFavorite), new { favoriteId = favorite?.Id });
    }

    /// <summary>
    /// Deletes a favorite for the authenticated user.
    /// </summary>
    /// <param name="favoriteId">The unique identifier (GUID) of the favorite.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Favorite deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to delete this favorite.</response>
    /// <response code="404">Favorite not found.</response>
    [HttpDelete("{favoriteId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFavorite(Guid favoriteId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authUserId))
            return HandleResponse<bool>(false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _favoriteService.DeleteFavoriteAsync(authUserId, favoriteId);
        return HandleResponse(success, statusCode, errorMessage);
    }
}
