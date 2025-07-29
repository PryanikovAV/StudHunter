using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Admin;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminController(AdminService administratorService) : BaseController
{
    private readonly AdminService _administratorService = administratorService;

    [HttpGet]
    public async Task<IActionResult> GetAllAdministrators()
    {
        var (administrators, statusCode, errorMessage) = await _administratorService.GetAllAdministratorsAsync();
        return this.CreateAPIError(administrators, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAdministrator(Guid id)
    {
        var (administrator, statusCode, errorMessage) = await _administratorService.GetAdministratorAsync(id);
        return this.CreateAPIError(administrator, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdministrator([FromBody] CreateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (administrator, statusCode, errorMessage) = await _administratorService.CreateAdministratorAsync(dto);
        return this.CreateAPIError(administrator, statusCode, errorMessage, nameof(GetAdministrator), new { id = administrator?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdministrator(Guid id, [FromBody] UpdateAdminDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (administrator, statusCode, errorMessage) = await _administratorService.UpdateAdministratorAsync(id, dto);
        return this.CreateAPIError<AdminDto>(administrator, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdministrator(Guid id, [FromQuery] bool hardDelete = false)
    {
        var (administrator, statusCode, errorMessage) = await _administratorService.DeletedministratorAsync(id, hardDelete);
        return this.CreateAPIError<AdminDto>(administrator, statusCode, errorMessage);
    }
}
