using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseVacancyService(StudHunterDbContext context) : BaseService(context)
{
    protected TDto MapToVacancyDto<TDto>(Vacancy vacancy) where TDto : VacancyDto, new()
    {
        var dto = new TDto
        {
            Id = vacancy.Id,
            EmployerId = vacancy.EmployerId,
            Title = vacancy.Title,
            Description = vacancy.Description,
            Salary = vacancy.Salary,
            CreatedAt = vacancy.CreatedAt,
            UpdatedAt = vacancy.UpdatedAt,
            Type = vacancy.Type.ToString()
        };

        if (dto is AdminVacancyDto adminVacancyDto)
        {
            adminVacancyDto.IsDeleted = vacancy.IsDeleted;
        }

        return dto;
    }

    /// <summary>
    /// Retrieves a vacancy by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the vacancy.</param>
    /// <returns>A tuple containing the vacancy, an optional status code, and an optional error message.</returns>
    public async Task<(VacancyDto? Entity, int? StatusCode, string? ErrorMessage)> GetVacancyAsync(Guid id)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        if (vacancy.IsDeleted)
            return (null, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Vacancy)));

        return (MapToVacancyDto<VacancyDto>(vacancy), null, null);
    }

    /// <summary>
    /// Retrieves all vacancies for a specific employer.
    /// </summary>
    /// <param name="employerId">The unique identifier (GUID) of the employer.</param>
    /// <returns>A tuple containing a list of vacancies, an optional status code, and an optional error message.</returns>
    public async Task<(List<VacancyDto>? Entities, int? StatusCode, string? ErrorMessage)> GetVacanciesByEmployerAsync(Guid employerId)
    {
        if (!await _context.Employers.AnyAsync(e => e.Id == employerId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        var vacancies = await _context.Vacancies
        .Where(v => v.EmployerId == employerId && !v.IsDeleted)
        .Select(v => MapToVacancyDto<VacancyDto>(v))
        .ToListAsync();

        return (vacancies, null, null);
    }

    /// <summary>
    /// Associates a course with a vacancy.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>A tuple indicating whether the operation was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> AddCourseToVacancyAsync(Guid vacancyId, Guid courseId)
    {
        if (!await _context.Vacancies.AnyAsync(v => v.Id == vacancyId))
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        if (!await _context.Courses.AnyAsync(c => c.Id == courseId))
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Course)));

        if (await _context.VacancyCourses.AnyAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId))
            return (false, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Course), nameof(Vacancy)));

        var vacancyCourse = new VacancyCourse
        {
            VacancyId = vacancyId,
            CourseId = courseId
        };

        _context.VacancyCourses.Add(vacancyCourse);

        return await SaveChangesAsync<VacancyCourse>();
    }

    /// <summary>
    /// Removes a course association from a vacancy.
    /// </summary>
    /// <param name="vacancyId">The unique identifier (GUID) of the vacancy.</param>
    /// <param name="courseId">The unique identifier (GUID) of the course.</param>
    /// <returns>A tuple indicating whether the operation was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> RemoveCourseFromVacancyAsync(Guid vacancyId, Guid courseId)
    {
        var vacancyCourse = await _context.VacancyCourses
        .FirstOrDefaultAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId);

        if (vacancyCourse == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.CannotDelete(nameof(Course), nameof(Vacancy)));

        _context.VacancyCourses.Remove(vacancyCourse);

        return await SaveChangesAsync<VacancyCourse>();
    }
}
