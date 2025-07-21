using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Administrator;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdministratorController(AdministratorService administratorService) : ControllerBase
{
    private readonly AdministratorService _administratorService = administratorService;

    [HttpGet]
    public async Task<IActionResult> GetAllAdministrators()
    {
        var administrators = await _administratorService.GetAllAdministratorsAsync();
        return Ok(administrators);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAdministrator(Guid id)
    {
        var administrator = await _administratorService.GetAdministratorAsync(id);
        if (administrator == null)
            return NotFound();
        return Ok(administrator);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdministrator([FromBody] CreateAdministratorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (administrator, error) = await _administratorService.CreateAdministratorAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetAdministrator), new { id = administrator!.Id }, administrator);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdministrator(Guid id, [FromBody] UpdateAdministratorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _administratorService.UpdateAdministratorAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    // TODO: add bool hardDelete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdministrator(Guid id)
    {
        var (success, error) = await _administratorService.DeletedministratorAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
