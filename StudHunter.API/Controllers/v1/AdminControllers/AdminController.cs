using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Admin;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminController(AdminService administratorService) : ControllerBase
{
    private readonly AdminService _administratorService = administratorService;

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
    public async Task<IActionResult> CreateAdministrator([FromBody] CreateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (administrator, error) = await _administratorService.CreateAdministratorAsync(dto);
        if (administrator == null)
            return error == null ? NotFound() : BadRequest(new { error });
        return CreatedAtAction(nameof(GetAdministrator), new { id = administrator!.Id }, administrator);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdministrator(Guid id, [FromBody] UpdateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _administratorService.UpdateAdministratorAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdministrator(Guid id, [FromQuery] bool hardDelete = false)
    {
        var (success, error) = await _administratorService.DeletedministratorAsync(id, hardDelete);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
