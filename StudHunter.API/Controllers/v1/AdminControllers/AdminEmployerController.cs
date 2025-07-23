using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminEmployerController(AdminEmployerService adminEmployerService) : ControllerBase
{
    private readonly AdminEmployerService _adminEmployerService = adminEmployerService;

    [HttpGet]
    public async Task<IActionResult> GetEmployers()
    {
        var employers = await _adminEmployerService.GetAllEmployersAsync();
        return Ok(employers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetEmployer(Guid id)
    {
        var employer = await _adminEmployerService.GetEmployerAsync(id);
        if (employer == null)
            return NotFound();
        return Ok(employer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployerByAdministrator(Guid id, [FromBody] UpdateEmployerByAdministratorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _adminEmployerService.UpdateEmployerAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployer(Guid id, [FromQuery] bool hardDelete = false)
    {
        var (success, error) = await _adminEmployerService.DeleteEmployerAsync(id, hardDelete);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
