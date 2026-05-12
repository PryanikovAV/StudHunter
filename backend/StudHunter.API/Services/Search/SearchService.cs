using Microsoft.EntityFrameworkCore;
using StudHunter.API.Extensions;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.Search;

public class SearchService(StudHunterDbContext context) : ISearchService
{
    public async Task<Result<ResumeSearchDto>> GetResumeByIdAsync(Guid resumeId, Guid? currentUserId = null)
    {
        var resume = await context.Resumes
            .AsNoTracking()
            .IncludeFullResumeDetails()
            .FirstOrDefaultAsync(r => r.Id == resumeId && !r.IsDeleted && !r.Student.IsDeleted);

        if (resume == null)
            return Result<ResumeSearchDto>.Failure(ErrorMessages.EntityNotFound(nameof(Resume)));

        bool maskContacts = true;

        if (currentUserId.HasValue)
        {
            var user = await context.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.RegistrationStage })
                .FirstOrDefaultAsync(u => u.Id == currentUserId.Value);

            if (user != null && user.RegistrationStage == User.AccountStatus.FullyActivated)
            {
                maskContacts = false;
            }
        }

        var dto = ResumeMapper.ToSearchDto(resume, maskContacts: maskContacts, false, false);
        return Result<ResumeSearchDto>.Success(dto);
    }

    public async Task<Result<PagedResult<VacancySearchDto>>> SearchVacanciesAsync(VacancySearchFilter filter, Guid? currentUserId = null)
    {
        var query = context.Vacancies
            .AsNoTracking()
            .Include(v => v.Employer).ThenInclude(e => e.City)
            .Include(v => v.Employer).ThenInclude(e => e.Specialization)
            .Include(v => v.Employer).ThenInclude(e => e.OrganizationDetails)
            .Include(v => v.Courses).ThenInclude(c => c.Course)
            .Include(v => v.AdditionalSkills).ThenInclude(s => s.AdditionalSkill)
            .Where(v => !v.IsDeleted && !v.Employer.IsDeleted
                     && v.Employer.RegistrationStage == User.AccountStatus.FullyActivated);

        if (currentUserId.HasValue)
        {
            var blockedEmployerIds = await context.BlackLists
                .Where(b => b.UserId == currentUserId.Value)
                .Select(b => b.BlockedUserId)
                .ToListAsync();

            if (blockedEmployerIds.Any())
            {
                query = query.Where(v => !blockedEmployerIds.Contains(v.EmployerId));
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(v =>
                EF.Functions.ILike(v.Title, $"%{term}%") ||
                EF.Functions.TrigramsAreSimilar(v.Title, term) ||
                EF.Functions.ILike(v.Employer.Name, $"%{term}%") ||
                EF.Functions.TrigramsAreSimilar(v.Employer.Name, term) ||
                EF.Functions.ILike(v.Description!, $"%{term}%"));
        }

        if (filter.EmployerId.HasValue)
            query = query.Where(v => v.EmployerId == filter.EmployerId.Value);

        if (filter.CityId.HasValue)
            query = query.Where(v => v.Employer.CityId == filter.CityId.Value);

        if (filter.SpecializationIds != null && filter.SpecializationIds.Any())
            query = query.Where(v => filter.SpecializationIds.Contains(v.Employer.SpecializationId!.Value));

        if (filter.VacancyTypes != null && filter.VacancyTypes.Any())
        {
            var typeEnums = filter.VacancyTypes
                .Select(t => Enum.TryParse<Vacancy.VacancyType>(t, out var type) ? type : (Vacancy.VacancyType?)null)
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToList();

            if (typeEnums.Any())
                query = query.Where(v => typeEnums.Contains(v.Type));
        }

        if (filter.CourseIds != null && filter.CourseIds.Any())
            query = query.Where(v => v.Courses.Any(c => filter.CourseIds.Contains(c.CourseId)));

        if (filter.SkillIds != null && filter.SkillIds.Any())
            query = query.Where(v => v.AdditionalSkills.Any(s => filter.SkillIds.Contains(s.AdditionalSkillId)));

        if (filter.MinSalary.HasValue)
            query = query.Where(v => v.Salary >= filter.MinSalary.Value);

        if (filter.OnlyWithSalary)
            query = query.Where(v => v.Salary != null && v.Salary > 0);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.OrderByDescending(v => EF.Functions.TrigramsSimilarity(v.Title, term));
        }
        else
        {
            query = query.OrderByDescending(v => v.CreatedAt);
        }

        var pagedEntities = await query.ToPagedResultAsync(filter.Paging);
        var dtos = pagedEntities.Items.Select(v => VacancyMapper.ToSearchDto(v, false, false)).ToList();

        return Result<PagedResult<VacancySearchDto>>.Success(new PagedResult<VacancySearchDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }

    public async Task<Result<PagedResult<ResumeSearchDto>>> SearchResumesAsync(
        ResumeSearchFilter filter,
        Guid? currentUserId = null,
        bool isAccreditedEmployer = false)
    {
        var query = context.Resumes
            .AsNoTracking()
            .IncludeFullResumeDetails()
            .Where(r => !r.IsDeleted && !r.Student.IsDeleted
                     && r.Student.RegistrationStage == User.AccountStatus.FullyActivated);

        if (currentUserId.HasValue)
        {
            var blockedStudentIds = await context.BlackLists
                .Where(b => b.UserId == currentUserId.Value)
                .Select(b => b.BlockedUserId)
                .ToListAsync();

            if (blockedStudentIds.Any())
            {
                query = query.Where(r => !blockedStudentIds.Contains(r.StudentId));
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(r =>
                EF.Functions.ILike(r.Title, $"%{term}%") ||
                EF.Functions.TrigramsAreSimilar(r.Title, term));
        }

        if (filter.CityId.HasValue)
            query = query.Where(r => r.Student.CityId == filter.CityId.Value);

        query = query.Where(r => r.Student.StudyPlan != null && !r.Student.StudyPlan.IsDeleted);

        if (filter.Statuses != null && filter.Statuses.Any())
        {
            var statusEnums = filter.Statuses
                .Select(s => Enum.TryParse<Student.StudentStatus>(s, out var stat) ? stat : (Student.StudentStatus?)null)
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .ToList();

            if (statusEnums.Any())
                query = query.Where(r => statusEnums.Contains(r.Student.Status));
        }

        if (filter.IsForeign.HasValue)
        {
            query = query.Where(r => r.Student.IsForeign == filter.IsForeign.Value);
        }

        if (filter.HasAvatar.HasValue)
        {
            if (filter.HasAvatar.Value)
            {
                query = query.Where(r => r.Student.AvatarUrl != null && r.Student.AvatarUrl != "");
            }
            else
            {
                query = query.Where(r => r.Student.AvatarUrl == null || r.Student.AvatarUrl == "");
            }
        }

        if (filter.StudyForms != null && filter.StudyForms.Any())
        {
            var formEnums = filter.StudyForms
                .Select(s => Enum.TryParse<StudyPlan.StudyPlanForm>(s, out var form) ? form : (StudyPlan.StudyPlanForm?)null)
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .ToList();

            if (formEnums.Any())
                query = query.Where(r => formEnums.Contains(r.Student.StudyPlan!.StudyForm));
        }

        if (filter.UniversityId.HasValue)
            query = query.Where(r => r.Student.StudyPlan!.UniversityId == filter.UniversityId.Value);

        if (filter.FacultyId.HasValue)
            query = query.Where(r => r.Student.StudyPlan!.FacultyId == filter.FacultyId.Value);

        if (filter.DepartmentId.HasValue)
            query = query.Where(r => r.Student.StudyPlan!.DepartmentId == filter.DepartmentId.Value);

        if (filter.StudyDirectionId.HasValue)
            query = query.Where(r => r.Student.StudyPlan!.StudyDirectionId == filter.StudyDirectionId.Value);

        if (filter.CourseNumber.HasValue)
            query = query.Where(r => r.Student.StudyPlan!.CourseNumber == filter.CourseNumber.Value);

        if (filter.SkillIds != null && filter.SkillIds.Any())
            query = query.Where(r => r.AdditionalSkills.Any(s => filter.SkillIds.Contains(s.AdditionalSkillId)));

        if (filter.CourseIds != null && filter.CourseIds.Any())
            query = query.Where(r => r.Student.StudyPlan!.StudyPlanCourses.Any(c => filter.CourseIds.Contains(c.CourseId)));

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.OrderByDescending(r => EF.Functions.TrigramsSimilarity(r.Title, term));
        }
        else
        {
            query = query.OrderByDescending(r => r.CreatedAt);
        }

        var pagedEntities = await query.ToPagedResultAsync(filter.Paging);

        var dtos = pagedEntities.Items.Select(r =>
            ResumeMapper.ToSearchDto(r, maskContacts: !isAccreditedEmployer, false, false)).ToList();

        return Result<PagedResult<ResumeSearchDto>>.Success(new PagedResult<ResumeSearchDto>(
            dtos, pagedEntities.TotalCount, pagedEntities.PageNumber, pagedEntities.PageSize));
    }
}