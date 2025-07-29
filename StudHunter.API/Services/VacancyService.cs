using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class VacancyService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    public UserAchievementService _userAchievementService = userAchievementService;

    public async Task<(List<VacancyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllVacanciesAsync()
    {
        var vacancies = await _context.Vacancies
        .Where(v => !v.IsDeleted)
        .Select(v => new VacancyDto
        {
            Id = v.Id,
            EmployerId = v.EmployerId,
            Title = v.Title,
            Description = v.Description,
            Salary = v.Salary,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            Type = v.Type.ToString()
        }).ToListAsync();

        return (vacancies, null, null);
    }

    public async Task<(VacancyDto? Entity, int? StatusCode, string? ErrorMessage)> GetVacancyAsync(Guid id)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        #region Serializers
        if (vacancy == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Vacancy"));

        if (vacancy.IsDeleted)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyDeleted("Vacancy"));
        #endregion

        return (new VacancyDto
        {
            Id = vacancy.Id,
            EmployerId = vacancy.EmployerId,
            Title = vacancy.Title,
            Description = vacancy.Description,
            Salary = vacancy.Salary,
            CreatedAt = vacancy.CreatedAt,
            UpdatedAt = vacancy.UpdatedAt,
            Type = vacancy.Type.ToString()
        }, null, null);
    }

    public async Task<(List<VacancyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetVacanciesByEmployerAsync(Guid id)
    {
        var vacancies = await _context.Vacancies
        .Where(e => e.EmployerId == id)
        .Select(v => new VacancyDto
        {
            Id = v.Id,
            EmployerId = v.EmployerId,
            Title = v.Title,
            Description = v.Description,
            Salary = v.Salary,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            Type = v.Type.ToString()
        }).ToListAsync();

        return (vacancies, null, null);
    }

    public async Task<(VacancyDto? Entity, int? StatusCode, string? ErrorMessage)> CreateVacancyAsync(Guid employerId, CreateVacancyDto dto)
    {
        #region Serializers
        var employer = await _context.Employers.FirstOrDefaultAsync(e => e.Id == employerId);
        if (employer == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Employer"));

        if (employer.AccreditationStatus == false)
            return (null, StatusCodes.Status404NotFound, "Employer is not accredited");
        #endregion

        var vacancy = new Vacancy
        {
            Id = Guid.NewGuid(),
            EmployerId = employerId,
            Title = dto.Title,
            Description = dto.Description,
            Salary = dto.Salary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Type = Enum.Parse<Vacancy.VacancyType>(dto.Type),
            IsDeleted = false
        };

        _context.Vacancies.Add(vacancy);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new VacancyDto
        {
            Id = vacancy.Id,
            EmployerId = vacancy.EmployerId,
            Title = vacancy.Title,
            Description = vacancy.Description,
            Salary = vacancy.Salary,
            CreatedAt = vacancy.CreatedAt,
            UpdatedAt = vacancy.UpdatedAt,
            Type = vacancy.Type.ToString()
        }, null, null);
    }

    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateVacancyAsync(Guid id, UpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        #region Serializers
        if (vacancy == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Vacancy"));
        if (vacancy.IsDeleted)
            return (false, StatusCodes.Status410Gone, ErrorMessages.AlreadyDeleted("Vacancy"));
        #endregion

        if (dto.Title != null)
            vacancy.Title = dto.Title;
        if (dto.Description != null)
            vacancy.Description = dto.Description;
        if (dto.Salary.HasValue)
            vacancy.Salary = dto.Salary;
        vacancy.UpdatedAt = DateTime.UtcNow;
        if (dto.Type != null)
            vacancy.Type = Enum.Parse<Vacancy.VacancyType>(dto.Type);
        vacancy.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> AddCourseToVacancyAsync(Guid vacancyId, Guid courseId)
    {
        #region Serializers
        var vacancyExists = await _context.Vacancies.AnyAsync(v => v.Id == vacancyId);
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
        var courseAssociated = await _context.VacancyCourses.AnyAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId);

        if (vacancyExists == false)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Vacancy"));

        if (courseExists == false)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Course"));

        if (courseAssociated)
            return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Course", "Vacancy"));
        #endregion

        var vacancyCourse = new VacancyCourse
        {
            VacancyId = vacancyId,
            CourseId = courseId
        };

        _context.VacancyCourses.Add(vacancyCourse);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> RemoveCourseFromVacancyAsync(Guid vacancyId, Guid courseId)
    {
        var vacancyCourse = await _context.VacancyCourses.FirstOrDefaultAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId);

        #region Serializers
        if (vacancyCourse == null)
            return (false, StatusCodes.Status404NotFound, "Course not associated with vacancy");
        #endregion

        _context.VacancyCourses.Remove(vacancyCourse);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Vacancy>();

        return (success, statusCode, errorMessage);
    }

    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteVacancyAsync(Guid id)
    {
        return await SoftDeleteEntityAsync<Vacancy>(id);
    }
}
