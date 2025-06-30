using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class StudentController(StudentService studentService) : ControllerBase
{
    private readonly StudentService _studentService = studentService;

    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _studentService.GetStudentsAsync();

        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetStudent(Guid id)
    {
        var student = await _studentService.GetStudentAsync(id);

        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        var (student, error) = await _studentService.CreateStudentAsync(dto);

        if (error != null)
            return Conflict(new { error });

        return CreatedAtAction(nameof(GetStudent), new { id = student!.Id }, student);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
    {
        var (success, error) = await _studentService.UpdateStudentAsync(id, dto);

        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var (success, error) = await _studentService.DeleteStudentAsync(id);

        if (!success)
            return error == null ? NotFound() : Conflict(new { error });

        return NoContent();
    }
}
