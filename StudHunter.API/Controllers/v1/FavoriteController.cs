using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class FavoriteController(FavoriteService favoriteService) : ControllerBase
{
    private readonly FavoriteService _favoriteService = favoriteService;

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetFavorites(Guid userId)
    {
        var favorites = await _favoriteService.GetFavoritesAsync(userId);
        return Ok(favorites);
    }
    // TODO: Replace Guid.NewGuid(); with User.FindFirstValue(ClaimTypes.NameIdentifier) after implementing JWT
    [HttpPost]
    public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = Guid.NewGuid();
        var (favorite, error) = await _favoriteService.CreateFavoriteAsync(userId, dto);
        if (favorite == null)
            return error == null ? NotFound() : BadRequest(new { error });
        return CreatedAtAction(nameof(GetFavorites), new { userId = favorite.UserId }, favorite);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var (success, error) = await _favoriteService.DeleteFavoriteAsync(id);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
