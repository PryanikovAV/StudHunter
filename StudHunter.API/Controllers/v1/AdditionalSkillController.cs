using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.ModelsDto.AdditionalSkillDto;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class AdditionalSkillController(AdditionalSkillService additionalSkillService) : BaseController
{
    private readonly AdditionalSkillService _additionalSkillService = additionalSkillService;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<AdditionalSkillDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAdditionalSkills()
    {
        var (skills, statusCode, errorMessage) = await _additionalSkillService.GetAllAdditionalSkillsAsync();
        return HandleResponse(skills, statusCode, errorMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    [HttpGet("{skillId}")]
    public async Task<IActionResult> GetAdditionalSkill(Guid skillId)
    {
        var (skill, statusCode, errorMessage) = await _additionalSkillService.GetAdditionalSkillAsync(skillId);
        return HandleResponse(skill, statusCode, errorMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillName"></param>
    /// <returns></returns>
    [HttpGet("skill/{skillName}")]
    public async Task<IActionResult> GetAdditionalSkill(string skillName)
    {
        var (skill, statusCode, errorMessage) = await _additionalSkillService.GetAdditionalSkillAsync(skillName);
        return HandleResponse(skill, statusCode, errorMessage);
    }
}
