using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Favorite;
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
    /// <response code="401">User is not authenticated or invalid user ID.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllFavorites()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return CreateAPIError<List<FavoriteDto>>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorites, statusCode, errorMessage) = await _favoriteService.GetAllFavoritesByUserAsync(userId);
        return CreateAPIError(favorites, statusCode, errorMessage);
    }

    /// <summary>
    /// Retrieves a favorite by its ID for the authenticated user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the favorite.</param>
    /// <returns>The favorite.</returns>
    /// <response code="200">Favorite retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Favorite not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFavorite(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return CreateAPIError<FavoriteDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorite, statusCode, errorMessage) = await _favoriteService.GetFavoriteAsync(id, userId);
        return CreateAPIError(favorite, statusCode, errorMessage);
    }

    /// <summary>
    /// Creates a new favorite for the authenticated user.
    /// </summary>
    /// <param name="dto">The data transfer object containing favorite details.</param>
    /// <returns>The created favorite.</returns>
    /// <response code="201">Favorite created successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Vacancy or resume not found.</response>
    /// <response code="409">A favorite with the specified userId and vacancyId/resumeId already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return CreateAPIError<FavoriteDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (favorite, statusCode, errorMessage) = await _favoriteService.CreateFavoriteAsync(userId, dto);
        return CreateAPIError(favorite, statusCode, errorMessage, nameof(GetFavorite), new { id = favorite?.Id });
    }

    /// <summary>
    /// Deletes a favorite for the authenticated user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the favorite.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Favorite deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Favorite not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return CreateAPIError<FavoriteDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var (success, statusCode, errorMessage) = await _favoriteService.DeleteFavoriteAsync(id, userId);
        return CreateAPIError<FavoriteDto>(success, statusCode, errorMessage);
    }
}
