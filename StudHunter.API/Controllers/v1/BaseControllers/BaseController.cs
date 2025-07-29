using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;

namespace StudHunter.API.Controllers.v1.BaseControllers;

public class BaseController : ControllerBase
{
    // GET
    protected IActionResult CreateAPIError<T>(T? entity, int? statusCode, string? errorMessage)
    {
        if (entity != null)
            return Ok(entity);
        return StatusCode(statusCode ?? StatusCodes.Status400BadRequest, new { error = errorMessage ?? ErrorMessages.InvalidData($"{typeof(T).Name.Replace("Dto", "").Replace("Admin", "")}") });
    }
    //POST
    protected IActionResult CreateAPIError<T>(T? entity, int? statusCode, string? errorMessage, string actionName, object routeValues)
    {
        if (entity != null)
            return CreatedAtAction(actionName, routeValues, entity);
        return StatusCode(statusCode ?? StatusCodes.Status400BadRequest, new { error = errorMessage ?? ErrorMessages.InvalidData($"{typeof(T).Name.Replace("Dto", "").Replace("Admin", "")}") });
    }
    // PUT, DELETE
    protected IActionResult CreateAPIError<T>(bool success, int? statusCode, string? errorMessage, string? entityName = null)
    {
        if (success)
            return NoContent();
        return StatusCode(statusCode ?? StatusCodes.Status400BadRequest, new { error = errorMessage ?? ErrorMessages.InvalidData($"{typeof(T).Name.Replace("Dto", "").Replace("Admin", "")}") });
    }
}
