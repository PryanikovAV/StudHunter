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
    /// <param name="statusCode">The HTTP status code for errors.</param>
    /// <param name="errorMessage">The error message for failed requests.</param>
    /// <returns>An IActionResult with status 200 OK if successful, or an error status code with an error message.</returns>
    protected IActionResult CreateAPIError<T>(T? entity, int? statusCode, string? errorMessage)
    {
        if (entity != null)
            return Ok(entity);
        return CreateErrorResponse(statusCode, errorMessage, GetEntityName<T>());
    }

    /// <summary>
    /// Creates an API response for POST requests.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to return.</param>
    /// <param name="statusCode">The HTTP status code for errors.</param>
    /// <param name="errorMessage">The error message for failed requests.</param>
    /// <param name="actionName">The name of the action to redirect to.</param>
    /// <param name="routeValues">The route values for the redirect.</param>
    /// <returns>An IActionResult with status 201 Created if successful, or an error status code with an error message.</returns>
    protected IActionResult CreateAPIError<T>(T? entity, int? statusCode, string? errorMessage, string actionName, object routeValues)
    {
        if (entity != null)
            return CreatedAtAction(actionName, routeValues, entity);
        return CreateErrorResponse(statusCode, errorMessage, GetEntityName<T>());
    }

    /// <summary>
    /// Creates an API response for PUT and DELETE requests.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="success">A boolean indicating if the operation was successful.</param>
    /// <param name="statusCode">The HTTP status code for errors.</param>
    /// <param name="errorMessage">The error message for failed requests.</param>
    /// <returns>An IActionResult with status 204 No Content if successful, or an error status code with an error message.</returns>
    protected IActionResult CreateAPIError<T>(bool success, int? statusCode, string? errorMessage)
    {
        if (success)
            return NoContent();
        return CreateErrorResponse(statusCode, errorMessage, GetEntityName<T>());
    }

    /// <summary>
    /// Creates an API response for validation errors.
    /// </summary>
    /// <returns>An IActionResult with status 400 Bad Request and validation error details.</returns>
    protected IActionResult ValidationError()
    {
        var errors = ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage)
        .ToList();

        var errorMessage = errors.Any() ? string.Join("; ", errors) : ErrorMessages.InvalidData("request data");

        return StatusCode(StatusCodes.Status400BadRequest, new { error = errorMessage });
    }

    private IActionResult CreateErrorResponse(int? statusCode, string? errorMessage, string entityName)
    {
        return StatusCode(statusCode ?? StatusCodes.Status500InternalServerError, new { error = errorMessage ?? ErrorMessages.InvalidData(entityName) });
    }

    private static string GetEntityName<T>(string? entityName = null)
    {
        return entityName ?? typeof(T).Name.Replace("Dto", "").Replace("Admin", "");
    }
}