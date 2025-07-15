using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Administrator;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class AdministratorController(AdministratorService administratorService) : ControllerBase
{
    private readonly AdministratorService _administratorService = administratorService;

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAdministrators()
    {
        var administrators = await _administratorService.GetAdministratorsAsync();
        return Ok(administrators);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> GetAdministrator(Guid id)
    {
        var administrator = await _administratorService.GetAdministratorAsync(id);
        if (administrator == null)
            return NotFound();
        return Ok(administrator);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateAdministrator([FromBody] CreateAdministratorDto dto)
    {
        var (administrator, error) = await _administratorService.CreateAdministratorAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetAdministrator), new { id = administrator!.Id }, administrator);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateAdministrator(Guid id, [FromBody] UpdateAdministratorDto dto)
    {
        var (success, error) = await _administratorService.UpdateAdministratorAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteAdministrator(Guid id)
    {
        var (success, error) = await _administratorService.DeleteAdministratorAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
