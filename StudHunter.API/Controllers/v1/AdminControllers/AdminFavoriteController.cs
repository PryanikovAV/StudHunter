using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminFavoriteController(AdminFavoriteService adminFavoriteService) : BaseController
{
    private readonly AdminFavoriteService _adminFavoriteService = adminFavoriteService;

    [HttpGet("favorites")]
    public async Task<IActionResult> GetAllFavorites()
    {
        var (favorites, statusCode, errorMessage) = await _adminFavoriteService.GetAllFavoritesAsync(););
        return this.CreateAPIError(favorites, statusCode, errorMessage);
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
