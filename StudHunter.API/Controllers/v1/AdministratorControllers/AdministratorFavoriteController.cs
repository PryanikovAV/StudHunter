using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class AdministratorFavoriteController(AdministratorFavoriteService administratorFavoriteService) : ControllerBase
{
    private readonly AdministratorFavoriteService _administratorFavoriteService = administratorFavoriteService;

    [HttpGet("favorites")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAllFavorites()
    {
        var favorites = await _administratorFavoriteService.GetAllFavoritesAsync();
        return Ok(favorites);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetFavorites(Guid userId)
    {
        var favorites = await _administratorFavoriteService.GetFavoritesAsync(userId);
        return Ok(favorites);
    }

    [HttpDelete("favorite/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var (success, error) = await _administratorFavoriteService.DeleteFavoriteAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
