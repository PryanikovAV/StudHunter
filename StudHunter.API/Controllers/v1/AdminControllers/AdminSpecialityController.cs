using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminSpecialityController(AdminSpecialityService adminSpecialityService) : BaseController
{
    private readonly AdminSpecialityService _adminSpecialityService = adminSpecialityService;

    [HttpGet]
    public async Task<IActionResult> GetAllSpecialties()
    {
        var (specialties, statusCode, errorMessage) = await _adminSpecialityService.GetAllSpecialtiesAsync();
        return this.CreateAPIError(specialties, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpeciality(Guid id)
    {
        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.GetSpecialityAsync(id);
        return this.CreateAPIError(speciality, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpeciality([FromBody] CreateSpecialityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.CreateSpecialityAsync(dto);
        return this.CreateAPIError(speciality, statusCode, errorMessage, nameof(GetSpeciality), new { id = speciality?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpeciality(Guid id, [FromBody] UpdateSpecialityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.UpdateSpecialityAsync(id, dto);
        return this.CreateAPIError<SpecialityDto>(speciality, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpeciality(Guid id)
    {
        var (speciality, statusCode, errorMessage) = await _adminSpecialityService.DeleteSpecialityAsync(id);
        return this.CreateAPIError<SpecialityDto>(speciality, statusCode, errorMessage);
    }
}
