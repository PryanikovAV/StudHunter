using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

/// <summary>
/// Controller for managing favorites with administrative privileges.
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminFavoriteController(AdminFavoriteService adminFavoriteService) : BaseController
{
    private readonly AdminFavoriteService _adminFavoriteService = adminFavoriteService;

    /// <summary>
    /// Retrieves all favorites for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user.</param>
    /// <returns>A list of favorites.</returns>
    /// <response code="200">Favorites retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllFavorites([FromQuery] Guid id)
    {
        var (favorites, statusCode, errorMessage) = await _adminFavoriteService.GetAllFavoritesByUserAsync(id);
        return CreateAPIError(favorites, statusCode, errorMessage);
    }

    /// <summary>
    /// Deletes a favorite.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the favorite.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Favorite deleted successfully.</response>
    /// <response code="400">Invalid request data or database error.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks Administrator role.</response>
    /// <response code="404">Favorite not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminFavoriteService.DeleteFavoriteAsync(id);
        return CreateAPIError<FavoriteDto>(success, statusCode, errorMessage);
    }
}
