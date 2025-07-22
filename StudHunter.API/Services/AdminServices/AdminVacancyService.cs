using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminVacancyService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<IEnumerable<VacancyDto>> GetAllVacanciesAsync()
    {
        return await _context.Vacancies.Select(v => new VacancyDto
        {
            Id = v.Id,
            EmployerId = v.EmployerId,
            Title = v.Title,
            Description = v.Description,
            Salary = v.Salary,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            Type = v.Type.ToString()
        })
        .ToListAsync();
    }

    public async Task<VacancyDto?> GetVacancyAsync(Guid id)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return null;

        return new VacancyDto
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
    }

    public async Task<(bool Success, string? Error)> UpdateVacancyAsync(Guid id, UpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return (false, "Vacancy not found.");

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

        return await SaveChangesAsync("update", "Vacancy");
    }

    public async Task<(bool Success, string? Error)> DeleteVacancyAsync(Guid id)
    {
        return await HardDeleteEntityAsync<Vacancy>(id);
    }

    public async Task<(bool Success, string? Error)> AddCourseToVacancyAsync(Guid vacancyId, Guid courseId)
    {
        if (!await _context.Vacancies.AnyAsync(v => v.Id == vacancyId))
            return (false, "Vacancy not found");

        if (!await _context.Courses.AllAsync(c => c.Id == courseId))
            return (false, "Course not found");

        if (await _context.VacancyCourses.AnyAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId))
            return (false, "Course already associated with vacancy");

        var vacancyCourse = new VacancyCourse
        {
            VacancyId = vacancyId,
            CourseId = courseId
        };

        _context.VacancyCourses.Add(vacancyCourse);

        return await SaveChangesAsync("add course to vacancy", "vacancy");
    }

    public async Task<(bool Success, string? Error)> RemoveCourseFromVacancyAsync(Guid vacancyId, Guid courseId)
    {
        var vacancyCourse = await _context.VacancyCourses.FirstOrDefaultAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId);

        if (vacancyCourse == null)
            return (false, "Course not associated with vacancy");

        _context.VacancyCourses.Remove(vacancyCourse);

        return await SaveChangesAsync("remove course from vacancy", "vacancy");
    }

    public async Task<(bool Success, string? Error)> SoftDeleteVacancyAsync(Guid id)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return (false, "Vacancy not found");

        vacancy.IsDeleted = true;

        return await SaveChangesAsync("soft delete", "Vacancy");
    }
}
