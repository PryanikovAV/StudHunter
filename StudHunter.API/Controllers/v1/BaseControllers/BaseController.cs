using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;

namespace StudHunter.API.Controllers.v1.BaseControllers;

/// <summary>
/// Base controller with common API error handling.
/// </summary>
public class BaseController : ControllerBase
{
    /// <summary>
    /// Creates an API response for GET requests.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to return.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>An IActionResult.</returns>
    protected IActionResult CreateAPIError<T>(T? entity, int? statusCode, string? errorMessage)
    {
        if (entity != null)
            return Ok(entity);
        return StatusCode(statusCode ?? StatusCodes.Status400BadRequest, new { error = errorMessage ?? ErrorMessages.InvalidData($"{typeof(T).Name.Replace("Dto", "").Replace("Admin", "")}") });
    }

    /// <summary>
    /// Creates an API response for POST requests.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to return.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="actionName">The name of the action to redirect to.</param>
    /// <param name="routeValues">The route values for the redirect.</param>
    /// <returns>An IActionResult.</returns>
    protected IActionResult CreateAPIError<T>(T? entity, int? statusCode, string? errorMessage, string actionName, object routeValues)
    {
        if (entity != null)
            return CreatedAtAction(actionName, routeValues, entity);
        return StatusCode(statusCode ?? StatusCodes.Status400BadRequest, new { error = errorMessage ?? ErrorMessages.InvalidData($"{typeof(T).Name.Replace("Dto", "").Replace("Admin", "")}") });
    }

    /// <summary>
    /// Creates an API response for PUT and DELETE requests.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="success">A boolean indicating if the operation was successful.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="entityName">The name of the entity.</param>
    /// <returns>An IActionResult.</returns>
    protected IActionResult CreateAPIError<T>(bool success, int? statusCode, string? errorMessage, string? entityName = null)
    {
        if (success)
            return NoContent();
        return StatusCode(statusCode ?? StatusCodes.Status400BadRequest, new { error = errorMessage ?? ErrorMessages.InvalidData($"{typeof(T).Name.Replace("Dto", "").Replace("Admin", "")}") });
    }
}
