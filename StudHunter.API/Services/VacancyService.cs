using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class VacancyService(StudHunterDbContext context)
{
    private readonly StudHunterDbContext _context = context;

    public async Task<IEnumerable<VacancyDto>> 
        GetVacanciesAsync()
    {
        return await _context.Vacancies
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
            })
            .ToListAsync();
    }

    public async Task<VacancyDto?> GetVacancyAsync(Guid id)
    {
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id);

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

    public async Task<(VacancyDto? Vacancy, string? Error)> 
        CreateVacancyAsync(CreateVacancyDto dto)
    {
        var employer = await _context.Employers
            .FirstOrDefaultAsync(e => e.Id == dto.EmployerId);

        if (employer == null)
            return (null, "Employer not found");

        if (!employer.AccreditationStatus)
            return (null, "Employer is not accredited");

        var vacancy = new Vacancy
        {
            Id = Guid.NewGuid(),
            EmployerId = dto.EmployerId,
            Title = dto.Title,
            Description = dto.Description,
            Salary = dto.Salary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Type = Enum.Parse<Vacancy.VacancyType>(dto.Type)
        };

        _context.Vacancies.Add(vacancy);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create vacancy: " +
                $"{ex.InnerException?.Message}");
        }

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
        }, null);
    }

    public async Task<(bool Success, string? Error)> 
        UpdateVacancyAsync(Guid id, UpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id);

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

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update vacancy: " +
                $"{ex.InnerException?.Message}");
        }
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> 
        DeleteVacancyAsync(Guid id)
    {
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return (false, "Vacancy not found");

        _context.Vacancies.Remove(vacancy);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete vacancy: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> 
        AddCourseToVacancyAsync(Guid vacancyId, Guid courseId)
    {
        if (!await _context.Vacancies.AnyAsync(v => v.Id == vacancyId))
            return (false, "Vacancy not found");

        if (!await _context.Courses.AllAsync(c => c.Id == courseId))
            return (false, "Course not found");

        if (await _context.VacancyCourses
            .AnyAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId))
            return (false, "Course already associated with vacancy");

        var vacancyCourse = new VacancyCourse
        {
            VacancyId = vacancyId,
            CourseId = courseId
        };

        _context.VacancyCourses.Add(vacancyCourse);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to add course to vacancy: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> 
        RemoveCourseFromVacancyAsync(Guid vacancyId, Guid courseId)
    {
        var vacancyCourse = await _context.VacancyCourses
            .FirstOrDefaultAsync(vc => vc.VacancyId == vacancyId && vc.CourseId == courseId);

        if (vacancyCourse == null)
            return (false, "Course not associated with vacancy");

        _context.VacancyCourses.Remove(vacancyCourse);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to remove course from vacancy: " +
                $"{ex.InnerException?.Message}");
        }
        return (true, null);
    }
}
