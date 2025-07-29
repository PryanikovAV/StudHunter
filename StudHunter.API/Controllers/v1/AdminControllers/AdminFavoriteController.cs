using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminFavoriteController(AdminFavoriteService adminFavoriteService) : BaseController
{
    private readonly AdminFavoriteService _adminFavoriteService = adminFavoriteService;

    [HttpGet("favorites")]
    public async Task<IActionResult> GetAllFavorites(Guid id)
    {
        var (favorites, statusCode, errorMessage) = await _adminFavoriteService.GetAllFavoritesAsync(id);
        return this.CreateAPIError(favorites, statusCode, errorMessage);
    }

    [HttpDelete("favorite/{id}")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var (favorites, statusCode, errorMessage) = await _adminFavoriteService.DeleteFavoriteAsync(id);
        return this.CreateAPIError<FavoriteDto>(favorites, statusCode, errorMessage);
    }
}
