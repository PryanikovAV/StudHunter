using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class AdministratorVacancyController(AdministratorVacancyService administratorVacancyService) : ControllerBase
    {
        private readonly AdministratorVacancyService _administratorVacancyService = administratorVacancyService;

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetVacancies()
        {
            var vacancies = await _administratorVacancyService.GetAllVacanciesAsync();
            return Ok(vacancies);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> GetVacancy(Guid id)
        {
            var vacancy = await _administratorVacancyService.GetVacancyAsync(id);
            if (vacancy == null)
                return NotFound();
            return Ok(vacancy);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] UpdateVacancyDto dto)
        {
            var (success, error) = await _administratorVacancyService.UpdateVacancyAsync(id, dto);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteVacancy(Guid id)
        {
            var (success, error) = await _administratorVacancyService.DeleteVacancyAsync(id);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }

        [HttpPost("{id}/courses")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddCourseToVacancy(Guid id, [FromBody] Guid courseId)
        {
            var (success, error) = await _administratorVacancyService.AddCourseToVacancyAsync(id, courseId);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }

        [HttpDelete("{id}/courses/{courseId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RemoveCourseFromVacancy(Guid id, Guid courseId)
        {
            var (success, error) = await _administratorVacancyService.RemoveCourseFromVacancyAsync(id, courseId);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }
    }
}
