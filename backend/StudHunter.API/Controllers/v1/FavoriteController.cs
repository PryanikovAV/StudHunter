using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1/favorites")]
public class FavoriteController(IFavoriteService favoriteService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyFavorites([FromQuery] PaginationParams paging) =>
        HandleResult(await favoriteService.GetMyFavoritesAsync(AuthorizedUserId, paging));

    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteRequest request) =>
        HandleResult(await favoriteService.ToggleFavoriteAsync(AuthorizedUserId, request));
}