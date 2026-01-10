using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1/blacklist")]
public class BlackListController(IBlackListService blackListService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyBlackList([FromQuery] PaginationParams paging) =>
        HandleResult(await blackListService.GetMyBlackListAsync(AuthorizedUserId, paging));

    [HttpPost("toggle/{blockedUserId:guid}")]
    public async Task<IActionResult> ToggleBlock(Guid blockedUserId, bool shouldBlock) =>
        HandleResult(await blackListService.ToggleBlockAsync(AuthorizedUserId, blockedUserId, shouldBlock));
}