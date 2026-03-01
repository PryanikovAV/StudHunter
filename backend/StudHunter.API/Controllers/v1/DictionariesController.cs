using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;
[Authorize]
[Route("api/v1/dictionaries")]
public class DictionariesController(IDictionariesService dictionariesService) : BaseController
{
    [HttpGet("universities")]
    public async Task<IActionResult> GetUniversities() =>
        HandleResult(await dictionariesService.GetUniversitiesAsync());

    [HttpGet("faculties")]
    public async Task<IActionResult> GetFaculties() =>
        HandleResult(await dictionariesService.GetFacultiesAsync());

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments() =>
        HandleResult(await dictionariesService.GetDepartmentsAsync());

    [HttpGet("specialities")]
    public async Task<IActionResult> GetStudyDirection() =>
        HandleResult(await dictionariesService.GetStudyDirectionAsync());

    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills() =>
        HandleResult(await dictionariesService.GetSkillsAsync());

    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses() =>
        HandleResult(await dictionariesService.GetAllCoursesAsync());

    [HttpGet("courses/search")]
    public async Task<IActionResult> SearchCourses([FromQuery] string q, [FromQuery] int limit = 10) =>
        HandleResult(await dictionariesService.SearchCoursesAsync(q, limit));

    [HttpGet("skills/search")]
    public async Task<IActionResult> SearchSkills([FromQuery] string q, [FromQuery] int limit = 10) =>
        HandleResult(await dictionariesService.SearchSkillsAsync(q, limit));

    [AllowAnonymous]
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities() =>
        HandleResult(await dictionariesService.GetCitiesAsync());

    [HttpGet("specializations")]
    public async Task<IActionResult> GetAllSpecializations() =>
        HandleResult(await dictionariesService.GetAllSpecializationsAsync());
}
