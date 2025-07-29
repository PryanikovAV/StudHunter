using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.AchievementTemplate;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Route("api/v1/admin/[controller]")]
[ApiController]
// [Authorize(Roles = "Administrator")]
public class AdminAchievementTemplateController(AdminAchievementTemplateService administratorAchievementTemplateService) : BaseController
{
    private readonly AdminAchievementTemplateService _administratorAchievementTemplateService = administratorAchievementTemplateService;

    [HttpGet]
    public async Task<IActionResult> GetAllAchievementTemplates()
    {
        var (templates, statusCode, errorMessage) = await _administratorAchievementTemplateService.GetAllAchievementTemplatesAsync();
        return this.CreateAPIError(templates, statusCode, errorMessage);
    }

    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> GetAchievementTemplate(int orderNumber)
    {
        var (template, statusCode, errorMessage) = await _administratorAchievementTemplateService.GetAchievementTemplateAsync(orderNumber);
        return this.CreateAPIError(template, statusCode, errorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAchievementTemplate([FromBody] CreateAchievementTemplateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (template, statusCode, errorMessage) = await _administratorAchievementTemplateService.CreateAchievementTemplateAsync(dto);
        return this.CreateAPIError(template, statusCode, errorMessage, nameof(GetAchievementTemplate), new { id = template?.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAchievementTemplate(Guid id, [FromBody] UpdateAchievementTemplateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (template, statusCode, errorMessage) = await _administratorAchievementTemplateService.UpdateAchievementTemplateAsync(id, dto);
        return this.CreateAPIError<AchievementTemplateDto>(template, statusCode, errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAchievementTemplate(Guid id)
    {
        var (template, statusCode, errorMessage) = await _administratorAchievementTemplateService.DeleteAchievementTemplateAsync(id);
        return this.CreateAPIError<AchievementTemplateDto>(template, statusCode, errorMessage);
    }
}
