using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.API.Services.AdministratorServices;

namespace StudHunter.API.Controllers.v1.AdministratorControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
public class AdministratorAchievementTemplateController(AdministratorAchievementTemplateService administratorAchievementTemplateService) : ControllerBase
{
    private readonly AdministratorAchievementTemplateService _administratorAchievementTemplateService = administratorAchievementTemplateService;

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAchievementTemplates()
    {
        var templates = await _administratorAchievementTemplateService.GetAllAchievementTemplatesAsync();
        return Ok(templates);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAchievementTemplate(int id)
    {
        var template = await _administratorAchievementTemplateService.GetAchievementTemplateAsync(id);
        if (template == null)
            return NotFound();
        return Ok(template);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateAchievementTemplate([FromBody] CreateAchievementTemplateDto dto)
    {
        var (template, error) = await _administratorAchievementTemplateService.CreateAchievementTemplateAsync(dto);
        if (error != null)
            return Conflict(new { error });
        return CreatedAtAction(nameof(GetAchievementTemplate), new { id = template!.Id }, template);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateAchievementTemplate(int id, [FromBody] UpdateAchievementTemplateDto dto)
    {
        var (success, error) = await _administratorAchievementTemplateService.UpdateAchievementTemplateAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteAchievementTemplate(int id)
    {
        var (success, error) = await _administratorAchievementTemplateService.DeleteAchievementTemplateAsync(id);
        if (!success)
            return error == null ? NotFound() : Conflict(new { error });
        return NoContent();
    }
}
