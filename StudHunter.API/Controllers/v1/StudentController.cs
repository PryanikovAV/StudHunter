using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Common;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Auth;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services;
using System.Security.Claims;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class StudentController(StudentService studentService) : BaseController
{
    private readonly StudentService _studentService = studentService;

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var (student, statusCode, errorMessage) = await _studentService.GetStudentAsync<StudentDto>(id);
        return CreateAPIError(student, statusCode, errorMessage);
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] RegisterStudentDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var (student, statusCode, errorMessage) = await _studentService.CreateStudentAsync(dto);
        return CreateAPIError(student, statusCode, errorMessage, nameof(GetStudent), new { id = student?.Id });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationError();

        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<StudentDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (userId != id)
            return CreateAPIError<StudentDto>(null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update", "profile"));

        var (success, statusCode, errorMessage) = await _studentService.UpdateStudentAsync(id, dto);
        return CreateAPIError<StudentDto>(success, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var userString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userString, out var userId))
            return CreateAPIError<StudentDto>(null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (userId != id)
            return CreateAPIError<StudentDto>(null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("delete", "profile"));

        var (success, statusCode, errorMessage) = await _studentService.DeleteStudentAsync(id);
        return CreateAPIError<StudentDto>(success, statusCode, errorMessage);
    }
}
