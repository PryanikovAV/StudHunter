using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.AdminServices;

namespace StudHunter.API.Controllers.v1.AdminControllers;

[Authorize(Roles = UserRoles.Administrator)]
[Route("api/v1/admin/dictionaries")]
public class AdminDictionariesController(IAdminDictionariesService adminDictionariesService) : BaseController
{
    [HttpGet("skills/all")]
    public async Task<IActionResult> GetAllSkills() =>
        HandleResult(await adminDictionariesService.GetAllSkillsAsync());

    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto dto) =>
        HandleResult(await adminDictionariesService.CreateSkillAsync(dto));

    [HttpPut("skills/{id:guid}")]
    public async Task<IActionResult> UpdateSkill(Guid id, [FromBody] UpdateSkillDto dto) =>
        HandleResult(await adminDictionariesService.UpdateSkillAsync(id, dto));

    [HttpDelete("skills/{id:guid}")]
    public async Task<IActionResult> DeleteSkill(Guid id) =>
        HandleResult(await adminDictionariesService.DeleteSkillAsync(id));
}