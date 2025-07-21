using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class FavoriteController(FavoriteService favoriteService) : ControllerBase
{
    private readonly FavoriteService _favoriteService = favoriteService;

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetFavorites(Guid userId)
    {
        var favorites = await _favoriteService.GetFavoritesAsync(userId);
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = Guid.NewGuid();  // <- Change this !!! (get from Jwt token)
        var (favorite, error) = await _favoriteService.CreateFavoriteAsync(userId, dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetFavorites),
            new { userId = favorite!.UserId }, favorite);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var (success, error) = await _favoriteService.DeleteFavoriteAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
