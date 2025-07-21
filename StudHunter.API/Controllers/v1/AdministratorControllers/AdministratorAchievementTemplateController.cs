using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdministratorAchievementTemplateController(AdministratorAchievementTemplateService administratorAchievementTemplateService) : ControllerBase
{
    private readonly AdministratorAchievementTemplateService _administratorAchievementTemplateService = administratorAchievementTemplateService;

    [HttpGet]
    public async Task<IActionResult> GetAllAchievementTemplates()
    {
        var templates = await _administratorAchievementTemplateService.GetAllAchievementTemplatesAsync();
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAchievementTemplate(int id)
    {
        var template = await _administratorAchievementTemplateService.GetAchievementTemplateAsync(id);
        if (template == null)
            return NotFound();
        return Ok(template);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAchievementTemplate([FromBody] CreateAchievementTemplateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (template, error) = await _administratorAchievementTemplateService.CreateAchievementTemplateAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetAchievementTemplate), new { id = template!.Id }, template);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAchievementTemplate(int id, [FromBody] UpdateAchievementTemplateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _administratorAchievementTemplateService.UpdateAchievementTemplateAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAchievementTemplate(int id)
    {
        var (success, error) = await _administratorAchievementTemplateService.DeleteAchievementTemplateAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
