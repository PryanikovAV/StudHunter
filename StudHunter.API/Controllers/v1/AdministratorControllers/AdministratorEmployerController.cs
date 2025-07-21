using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdministratorEmployerController(AdministratorEmployerService administratorEmployerService) : ControllerBase
{
    private readonly AdministratorEmployerService _administratorEmployerService = administratorEmployerService;

    [HttpGet]
    public async Task<IActionResult> GetEmployers()
    {
        var employers = await _administratorEmployerService.GetAllEmployersAsync();
        return Ok(employers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetEmployer(Guid id)
    {
        var employer = await _administratorEmployerService.GetEmployerAsync(id);
        if (employer == null)
            return NotFound();
        return Ok(employer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployerByAdministrator(Guid id, [FromBody] UpdateEmployerByAdministratorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _administratorEmployerService.UpdateEmployerAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    // TODO: add bool hardDelete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployer(Guid id)
    {
        var (success, error) = await _administratorEmployerService.DeleteEmployerAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
