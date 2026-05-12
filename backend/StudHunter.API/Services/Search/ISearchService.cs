using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;

namespace StudHunter.API.Services.Search;

public interface ISearchService
{
    Task<Result<ResumeSearchDto>> GetResumeByIdAsync(Guid resumeId, Guid? currentUserId = null);
    Task<Result<PagedResult<VacancySearchDto>>> SearchVacanciesAsync(VacancySearchFilter filter, Guid? currentUserId = null);
    Task<Result<PagedResult<ResumeSearchDto>>> SearchResumesAsync(ResumeSearchFilter filter, Guid? currentUserId = null, bool isAccreditedEmployer = false);
}
