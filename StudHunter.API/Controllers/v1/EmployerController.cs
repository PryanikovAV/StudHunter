using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class EmployerController(EmployerService employerService) : BaseController
{
    private readonly EmployerService _employerService = employerService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployer(Guid id)
    {
        var (employer, statusCode, errorMessage) = await _employerService.GetEmployerAsync(id);
        return this.CreateAPIError(employer, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployer([FromBody] CreateEmployerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (employer, statusCode, errorMessage) = await _employerService.CreateEmployerAsync(dto);
        return this.CreateAPIError(employer, statusCode, errorMessage, nameof(GetEmployer), new { id = employer?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] UpdateEmployerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (success, statusCode, errorMessage) = await _employerService.UpdateEmployerAsync(id, dto);
        return this.CreateAPIError<EmployerDto>(success, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEmployer(Guid id)
    {
        var (success, statusCode, errorMessage) = await _employerService.DeleteEmployerAsync(id);
        return this.CreateAPIError<EmployerDto>(success, statusCode, errorMessage);
    }
}
