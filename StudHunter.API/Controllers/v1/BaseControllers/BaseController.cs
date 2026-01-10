using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Infrastructure;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1.BaseControllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class BaseController : ControllerBase
{
    protected Guid AuthorizedUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (!result.IsSuccess)
            return Problem(detail: result.ErrorMessage, statusCode: result.StatusCode);

        if (result.Value is null || (result.Value is bool b && b == false && result.StatusCode == StatusCodes.Status204NoContent))
            return NoContent();

        return Ok(result.Value);
    }
}