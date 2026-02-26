using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/users/{userId:guid}/favorites")]
public class AdminFavoriteController(IAdminFavoriteService adminFavoriteService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUserFavorites(Guid userId, [FromQuery] PaginationParams paging) =>
        HandleResult(await adminFavoriteService.GetUserFavoritesAsync(userId, paging));
}