using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminFavoriteController(AdminFavoriteService adminFavoriteService) : ControllerBase
{
    private readonly AdminFavoriteService _adminFavoriteService = adminFavoriteService;

    [HttpGet("favorites")]
    public async Task<IActionResult> GetAllFavorites()
    {
        var favorites = await _adminFavoriteService.GetAllFavoritesAsync();
        return Ok(favorites);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetFavorites(Guid userId)
    {
        var favorites = await _adminFavoriteService.GetFavoritesAsync(userId);
        return Ok(favorites);
    }

    [HttpDelete("favorite/{id}")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var (success, error) = await _adminFavoriteService.DeleteFavoriteAsync(id);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
