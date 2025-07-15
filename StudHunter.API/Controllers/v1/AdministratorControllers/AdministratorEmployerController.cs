using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class AdministratorEmployerController(AdministratorEmployerService administratorEmployerService) : ControllerBase
    {
        private readonly AdministratorEmployerService _administratorEmployerService = administratorEmployerService;

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetEmployers()
        {
            var employers = await _administratorEmployerService.GetAllEmployersAsync();
            return Ok(employers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> GetEmployer(Guid id)
        {
            var employer = await _administratorEmployerService.GetEmployerAsync(id);
            if (employer == null)
                return NotFound();
            return Ok(employer);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateEmployerByAdministrator(Guid id, [FromBody] UpdateEmployerByAdministratorDto dto)
        {
            var (success, error) = await _administratorEmployerService.UpdateEmployerAsync(id, dto);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteEmployer(Guid id)
        {
            var (success, error) = await _administratorEmployerService.DeleteEmployerAsync(id);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }
    }
}
