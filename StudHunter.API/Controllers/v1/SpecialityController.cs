using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class SpecialityController(SpecialityService specialityService) : BaseController
{
    private readonly SpecialityService _specialityService = specialityService;

    [HttpGet]
    public async Task<IActionResult> GetSpecialities()
    {
        var (specialities, statusCode, errorMessage) = await _specialityService.GetAllSpecialtiesAsync();
        return this.CreateAPIError(specialities, statusCode, errorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpeciality(Guid id)
    {
        var (speciality, statusCode, errorMessage) = await _specialityService.GetSpecialityAsync(id);
        return this.CreateAPIError(speciality, statusCode, errorMessage);
    }
}
