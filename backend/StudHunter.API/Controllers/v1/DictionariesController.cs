using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Authorize]
[Route("api/v1/dictionaries")]
public class DictionariesController(IDictionariesService dictionariesService) : BaseController
{
    [HttpGet("faculties")]
    public async Task<IActionResult> GetFaculties() =>
        HandleResult(await dictionariesService.GetFacultiesAsync());

    [HttpGet("specialities")]
    public async Task<IActionResult> GetSpecialities() =>
        HandleResult(await dictionariesService.GetSpecialitiesAsync());

    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills() =>
        HandleResult(await dictionariesService.GetSkillsAsync());

    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses() =>
        HandleResult(await dictionariesService.GetCoursesAsync());
}