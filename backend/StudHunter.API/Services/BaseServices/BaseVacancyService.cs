using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseVacancyService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : BaseService(context, registrationManager)
{
    protected IQueryable<Vacancy> GetFullVacancyQuery() =>
        _context.Vacancies
            .Include(v => v.Employer)
            .Include(v => v.AdditionalSkills).ThenInclude(s => s.AdditionalSkill)
            .Include(v => v.Courses).ThenInclude(c => c.Course);

    public async Task<Result<VacancyDto>> GetVacancyById(Guid vacancyId, Guid? currentUserId = null, bool ignoreFilters = true)
    {
        var query = GetFullVacancyQuery();

        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        var vacancy = await query.FirstOrDefaultAsync(v => v.Id == vacancyId);
        if (vacancy == null)
            return Result<VacancyDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        bool isBlocked = false;
        User.AccountStatus userStatus = User.AccountStatus.Anonymous;

        if (currentUserId.HasValue)
        {
            var blackListCheck = await EnsureCommunicationAllowedAsync(currentUserId.Value, vacancy.EmployerId);
            if (!blackListCheck.IsSuccess)
                isBlocked = true;  // Отметка для Dto

            var currentUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == currentUserId.Value);
            userStatus = currentUser?.RegistrationStage ?? User.AccountStatus.Anonymous;
        }

        return Result<VacancyDto>.Success(VacancyMapper.ToDto(vacancy, isBlocked, userStatus));
    }

    public async Task<Result<PagedResult<VacancyDto>>> GetAllVacanciesAsync(Guid employerId, PaginationParams paging, bool ignoreFilters = false)
    {
        var query = GetFullVacancyQuery();
        if (ignoreFilters)
            query = query.IgnoreQueryFilters();

        var pagedVacancies = await query
            .Where(v => v.EmployerId == employerId)
            .OrderByDescending(v => v.CreatedAt)
            .ToPagedResultAsync(paging);

        var dtos = pagedVacancies.Items
            .Select(v => VacancyMapper.ToDto(v))
            .ToList();

        var pagedResult = new PagedResult<VacancyDto>(
            Items: dtos,
            TotalCount: pagedVacancies.TotalCount,
            PageNumber: pagedVacancies.PageNumber,
            PageSize: pagedVacancies.PageSize);

        return Result<PagedResult<VacancyDto>>.Success(pagedResult);
    }

    public async Task<Result<VacancyDto>> CreateVacancyAsync(Guid employerId, UpdateVacancyDto dto)
    {
        var employer = await _context.Employers.FirstOrDefaultAsync(e => e.Id == employerId);
        if (employer == null)
            return Result<VacancyDto>.Failure(ErrorMessages.EntityNotFound(nameof(Employer)), StatusCodes.Status404NotFound);

        var permission = EnsureCanPerform(employer, UserAction.CreateVacancy);
        if (!permission.IsSuccess)
            return Result<VacancyDto>.Failure(permission.ErrorMessage!, permission.StatusCode);

        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<VacancyDto>.Failure("Заголовок вакансии обязателен для заполнения", StatusCodes.Status400BadRequest);

        if (!dto.Type.HasValue)
            return Result<VacancyDto>.Failure("Тип вакансии (Работа/Стажировка) обязателен", StatusCodes.Status400BadRequest);

        var vacancy = new Vacancy
        {
            EmployerId = employerId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Salary = dto.Salary,
            Type = dto.Type.Value
        };

        if (dto.SkillIds?.Any() == true)
            vacancy.AdditionalSkills = dto.SkillIds.Select(id => new VacancyAdditionalSkill { AdditionalSkillId = id }).ToList();

        if (dto.CourseIds?.Any() == true)
            vacancy.Courses = dto.CourseIds.Select(id => new VacancyCourse { CourseId = id }).ToList();

        _context.Vacancies.Add(vacancy);

        _registrationManager.RecalculateRegistrationStage(employer);

        var result = await SaveChangesAsync<Vacancy>();

        return result.IsSuccess
            ? await GetVacancyById(vacancy.Id)
            : Result<VacancyDto>.Failure(result.ErrorMessage!);
    }

    protected private void UpdateRelatedEntities(Vacancy vacancy, List<Guid>? newSkillIds, List<Guid>? newCourseIds)
    {
        if (newSkillIds != null)
        {
            var skillsToRemove = vacancy.AdditionalSkills
                .Where(s => !newSkillIds.Contains(s.AdditionalSkillId))
                .ToList();

            foreach (var toRemove in skillsToRemove)
                vacancy.AdditionalSkills.Remove(toRemove);

            var currentSkillIds = vacancy.AdditionalSkills
                .Select(s => s.AdditionalSkillId)
                .ToList();

            var skillsToAdd = newSkillIds.Except(currentSkillIds);

            foreach (var id in skillsToAdd)
                vacancy.AdditionalSkills.Add(new VacancyAdditionalSkill { AdditionalSkillId = id });
        }

        if (newCourseIds != null)
        {
            var coursesToRemove = vacancy.Courses
                .Where(c => !newCourseIds.Contains(c.CourseId))
                .ToList();

            foreach (var toRemove in coursesToRemove)
                vacancy.Courses.Remove(toRemove);

            var currentCourseIds = vacancy.Courses
                .Select(c => c.CourseId)
                .ToList();

            var coursesToAdd = newCourseIds.Except(currentCourseIds);

            foreach (var id in coursesToAdd)
                vacancy.Courses.Add(new VacancyCourse { CourseId = id });
        }
    }
}
