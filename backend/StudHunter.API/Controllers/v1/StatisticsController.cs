using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.StatisticsService;

namespace StudHunter.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class StatisticsController(IStatisticsService statisticsService) : BaseController
{
    [HttpGet("general")]
    [AllowAnonymous]
    public async Task<ActionResult<GeneralStatisticsDto>> GetGeneralStatistics(CancellationToken cancellationToken)
    {
        var stats = await statisticsService.GetGeneralStatisticsAsync(cancellationToken);
        return Ok(stats);
    }
}
