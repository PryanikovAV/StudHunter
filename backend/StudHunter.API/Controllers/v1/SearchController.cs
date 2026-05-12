using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.Pdf;
using StudHunter.API.Services.Search;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
public class SearchController(ISearchService searchService) : BaseController
{
    [HttpPost("vacancies")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchVacancies([FromBody] VacancySearchFilter filter)
    {
        var userId = User.Identity?.IsAuthenticated == true ? AuthorizedUserId : (Guid?)null;
        return HandleResult(await searchService.SearchVacanciesAsync(filter, userId));
    }

    [HttpPost("resumes")]
    [Authorize(Roles = UserRoles.Employer)]
    public async Task<IActionResult> SearchResumes([FromBody] ResumeSearchFilter filter)
    {
        return HandleResult(await searchService.SearchResumesAsync(filter, AuthorizedUserId, isAccreditedEmployer: true));
    }

    [HttpGet("resumes/{resumeId:guid}/pdf")]
    [Authorize]
    public async Task<IActionResult> DownloadResumePdf(Guid resumeId, [FromServices] ISearchService searchService, [FromServices] IPdfService pdfService)
    {
        var result = await searchService.GetResumeByIdAsync(resumeId, AuthorizedUserId);

        if (!result.IsSuccess)
            return HandleResult(result);

        var pdfBytes = pdfService.GenerateResumePdf(result.Value);
        var fileName = $"Resume_{result.Value.FullName.Replace(" ", "_")}.pdf";
        
        return File(pdfBytes, "application/pdf", fileName);
    }
}
