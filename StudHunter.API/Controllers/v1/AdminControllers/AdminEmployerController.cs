using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminEmployerController(AdminEmployerService adminEmployerService) : BaseController
{
    private readonly AdminEmployerService _adminEmployerService = adminEmployerService;

    [HttpGet]
    public async Task<IActionResult> GetAllEmployers()
    {
        var (employers, statusCode, errorMessage) = await _adminEmployerService.GetAllEmployersAsync();
        return this.CreateAPIError(employers, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployer(Guid id)
    {
        var (employer, statusCode, errorMessage) = await _adminEmployerService.GetEmployerAsync(id);
        return this.CreateAPIError(employer, statusCode, errorMessage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] AdminUpdateEmployerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (success, statusCode, errorMessage) = await _adminEmployerService.UpdateEmployerAsync(id, dto);
        return this.CreateAPIError<AdminEmployerDto>(success, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployer(Guid id)
    {
        var (success, statusCode, errorMessage) = await _adminEmployerService.DeleteEmployerAsync(id);
        return this.CreateAPIError<AdminEmployerDto>(success, statusCode, errorMessage);
    }
}
