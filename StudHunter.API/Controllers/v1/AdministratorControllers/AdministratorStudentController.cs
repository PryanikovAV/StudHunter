using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class AdministratorStudentController(AdministratorStudentService administratorStudentService) : ControllerBase
    {
        private readonly AdministratorStudentService _administratorStudentService = administratorStudentService;

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _administratorStudentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> GetStudent(Guid id)
        {
            var student = await _administratorStudentService.GetStudentAsync(id);
            if (student == null)
                return NotFound();
            return Ok(student);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentByAdministratorDto dto)
        {
            var (success, error) = await _administratorStudentService.UpdateStudentAsync(id, dto);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var (success, error) = await _administratorStudentService.DeleteStudentAsync(id);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }
    }
}
