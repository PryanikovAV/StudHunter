using Microsoft.AspNetCore.Mvc;

namespace StudHunter.API.Controllers.v1.BaseControllers;

/// <summary>
/// Base controller with common API error handling.
/// </summary>
public class BaseController : ControllerBase
{
    protected bool ValidateModel()
    {
        return ModelState.IsValid;
    }

    // GET, POST
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="statusCode"></param>
    /// <param name="errorMessage"></param>
    /// <param name="actionName"></param>
    /// <param name="routeValues"></param>
    /// <returns></returns>
    protected IActionResult HandleResponse<T>(T? entity, int? statusCode, string? errorMessage, string? actionName = null, object? routeValues = null)
    {
        if (statusCode.HasValue)
            return StatusCode(statusCode.Value, new { error = errorMessage, errorCode = statusCode.Value });

        if (!string.IsNullOrEmpty(actionName))
            return CreatedAtAction(actionName, routeValues, entity);
        
        return Ok(entity);
    }

    // PUT, DELETE
    /// <summary>
    /// 
    /// </summary>
    /// <param name="success"></param>
    /// <param name="statusCode"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    protected IActionResult HandleResponse(bool success, int? statusCode, string? errorMessage)
    {
        if (!success && statusCode.HasValue)
            return StatusCode(statusCode.Value, new { error = errorMessage, errorCode = statusCode.Value});
        return NoContent();
    }
}
