using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class FavoriteController(FavoriteService favoriteService) : BaseController
{
    private readonly FavoriteService _favoriteService = favoriteService;

    [HttpGet("favorites")]
    public async Task<IActionResult> GetAllFavorites()
    {
        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (favorites, statusCode, errorMessage) = await _favoriteService.GetAllFavoritesAsync(userId);
        return this.CreateAPIError(favorites, statusCode, errorMessage);
    }

    [HttpGet("favorites/{id}")]
    public async Task<IActionResult> GetFavorite(Guid id)
    {
        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (favorite, statusCode, errorMessage) = await _favoriteService.GetFavoriteAsync(id, userId);
        return this.CreateAPIError(favorite, statusCode, errorMessage);
    }


    [HttpPost]
    public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (favorite, statusCode, errorMessage) = await _favoriteService.CreateFavoriteAsync(userId, dto);
        return this.CreateAPIError(favorite, statusCode, errorMessage, nameof(GetFavorite), new { id = favorite?.Id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var userId = Guid.NewGuid();  // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
        var (success, statusCode, errorMessage) = await _favoriteService.DeleteFavoriteAsync(id, userId);
        return this.CreateAPIError<FavoriteDto>(success, statusCode, errorMessage);
    }
}
