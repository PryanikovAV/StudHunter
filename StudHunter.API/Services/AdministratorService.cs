using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Administrator;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.ModelsDto.Course;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.API.ModelsDto.Resume;
using StudHunter.API.ModelsDto.Student;
using StudHunter.API.ModelsDto.Vacancy;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class AdministratorService
    (StudHunterDbContext context, IPasswordHasher passwordHasher)
{
    private readonly StudHunterDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<IEnumerable<AdministratorDto>> GetAdministratorsAsync()
    {
        return await _context.Administrators
            .Select(a => new AdministratorDto
            {
                Id = a.Id,
                Email = a.Email,
                ContactEmail = a.ContactEmail,
                ContactPhone = a.ContactPhone,
                CreatedAt = a.CreatedAt,
                FirstName = a.FirstName,
                LastName = a.LastName,
                AdminLevel = a.AdminLevel.ToString()
            })
            .ToListAsync();
    }

    public async Task<AdministratorDto?> GetAdministratorAsync(Guid id)
    {
        var administrator = await _context.Administrators
            .FirstOrDefaultAsync(a => a.Id == id);

        if (administrator == null)
            return null;

        return new AdministratorDto
        {
            Id = administrator.Id,
            Email = administrator.Email,
            ContactEmail = administrator.ContactEmail,
            ContactPhone = administrator.ContactPhone,
            CreatedAt = administrator.CreatedAt,
            FirstName = administrator.FirstName,
            LastName = administrator.LastName,
            AdminLevel = administrator.AdminLevel.ToString()
        };
    }

    public async Task<(AdministratorDto? Administrator, string? Error)>
        CreateAdministratorAsync(CreateAdministratorDto dto)
    {
        if (await _context.Administrators.AnyAsync(e => e.Email == dto.Email))
            return (null, "Administrator with this email already exists");

        var administrator = new Administrator
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            CreatedAt = DateTime.UtcNow,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            AdminLevel = Enum.Parse<Administrator.AdministratorLevel>(dto.AdminLevel)
        };

        _context.Administrators.Add(administrator);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create administrator:" +
                $" {ex.InnerException?.Message}");
        }

        return (new AdministratorDto
        {
            Id = administrator.Id,
            Email = administrator.Email,
            ContactEmail = administrator.ContactEmail,
            ContactPhone = administrator.ContactPhone,
            CreatedAt = administrator.CreatedAt,
            FirstName = administrator.FirstName,
            LastName = administrator.LastName,
            AdminLevel = administrator.AdminLevel.ToString()
        }, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateAdministratorAsync(Guid id, UpdateAdministratorDto dto)
    {
        var administrator = await _context.Administrators
            .FirstOrDefaultAsync(a => a.Id == id);

        if (administrator == null)
            return (false, "Administrator not found");

        if (dto.Email != null && await _context.Administrators
            .AnyAsync(e => e.Email == dto.Email && e.Id != id))
            return (false, "Administrator with this email already exists");

        if (dto.Email != null)
            administrator.Email = dto.Email;
        if (dto.Password != null)
            administrator.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        if (dto.ContactEmail != null)
            administrator.ContactEmail = dto.ContactEmail;
        if (dto.ContactPhone != null)
            administrator.ContactPhone = dto.ContactPhone;
        if (dto.FirstName != null)
            administrator.FirstName = dto.FirstName;
        if (dto.LastName != null)
            administrator.LastName = dto.LastName;
        if (dto.AdminLevel != null)
            administrator.AdminLevel = Enum.Parse<Administrator.
                AdministratorLevel>(dto.AdminLevel);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update administrator: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)>
        DeleteAdministratorAsync(Guid id)
    {
        var administrator = await _context.Administrators
            .FirstOrDefaultAsync(a => a.Id == id);

        if (administrator == null)
            return (false, "Administrator not found");

        if (administrator.AdminLevel == Administrator.AdministratorLevel.SuperAdmin)
            return (false, "Cannot delete SuperAdmin");

        _context.Administrators.Remove(administrator);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete administrator: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateEmployerAccreditationAsync(Guid id, bool accreditationStatus)
    {
        var employer = await _context.Employers
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employer == null)
            return (false, "Employer not found");

        employer.AccreditationStatus = accreditationStatus;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update employer accreditation: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateStudentAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _context.Students
            .Include(s => s.StudyPlan)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return (false, "Student not found");

        if (dto.Email != null && await _context.Students
            .AnyAsync(s => s.Email == dto.Email && s.Id != id))
            return (false, "Another student with this email already exists");

        if (dto.FacultyId.HasValue && !await _context.Faculties
            .AnyAsync(f => f.Id == dto.FacultyId.Value))
            return (false, "Faculty not found");

        if (dto.SpecialityId.HasValue && !await _context.Specialities
            .AnyAsync(s => s.Id == dto.SpecialityId.Value))
            return (false, "Speciality not found");

        if (dto.FirstName != null)
            student.FirstName = dto.FirstName;
        if (dto.LastName != null)
            student.LastName = dto.LastName;
        if (dto.Email != null)
            student.Email = dto.Email;
        if (dto.Gender != null)
            student.Gender = Enum.Parse<Student.StudentGender>(dto.Gender);
        if (dto.BirthDate.HasValue)
            student.BirthDate = dto.BirthDate.Value;
        if (dto.Photo != null)
            student.Photo = dto.Photo;
        if (dto.ContactPhone != null)
            student.ContactPhone = dto.ContactPhone;
        if (dto.ContactEmail != null)
            student.ContactEmail = dto.ContactEmail;
        if (dto.IsForeign.HasValue)
            student.IsForeign = dto.IsForeign.Value;

        if (dto.CourseNumber.HasValue || dto.FacultyId.HasValue ||
            dto.SpecialityId.HasValue || dto.StudyForm != null || dto.BeginYear.HasValue)
        {
            var studyPlan = student.StudyPlan;
            if (dto.CourseNumber.HasValue)
                studyPlan.CourseNumber = dto.CourseNumber.Value;
            if (dto.FacultyId.HasValue)
                studyPlan.FacultyId = dto.FacultyId.Value;
            if (dto.SpecialityId.HasValue)
                studyPlan.SpecialityId = dto.SpecialityId.Value;
            if (dto.StudyForm != null)
                studyPlan.StudyForm = Enum.Parse<StudyPlan.StudyForms>(dto.StudyForm);
            if (dto.BeginYear.HasValue)
                studyPlan.BeginYear = dto.BeginYear.Value;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update student: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateEmployerAsync(Guid id, UpdateEmployerDto dto)
    {
        var employer = await _context.Employers
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employer == null)
            return (false, "Employer not found");

        if (dto.Email != null && await _context.Employers
            .AnyAsync(e => e.Email == dto.Email && e.Id != id))
            return (false, "Another employer with this email already exists");

        if (dto.ContactEmail != null)
            employer.ContactEmail = dto.ContactEmail;
        if (dto.ContactPhone != null)
            employer.ContactPhone = dto.ContactPhone;
        if (dto.Name != null)
            employer.Name = dto.Name;
        if (dto.Description != null)
            employer.Description = dto.Description;
        if (dto.Website != null)
            employer.Website = dto.Website;
        if (dto.Specialization != null)
            employer.Specialization = dto.Specialization;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update employer: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateResumeAsync(Guid id, UpdateResumeDto dto)
    {
        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r => r.Id == id);

        if (resume == null)
            return (false, "Resume not found");
        if (dto.Title != null)
            resume.Title = dto.Title;
        if (dto.Description != null)
            resume.Description = dto.Description;
        resume.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update resume:" +
                $" {ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateVacancyAsync(Guid id, UpdateVacancyDto dto)
    {
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vacancy == null)
            return (false, "Vacancy not found");

        if (dto.Title != null)
            vacancy.Title = dto.Title;
        if (dto.Description != null)
            vacancy.Description = dto.Description;
        if (dto.Salary.HasValue)
            vacancy.Salary = dto.Salary.Value;
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

    public async Task<(CourseDto? Course, string? Error)>
        CreateCourseAync(CreateCourseDto dto)
    {
        if (await _context.Courses.AnyAsync(c => c.Name == dto.Name))
            return (null, "Course with this name already exists");

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Courses.Add(course);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create course: " +
                $"{ex.InnerException?.Message}");
        }

        return (new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        }, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateCourseAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return (false, "Course not found");

        if (dto.Name != null && await _context.Courses
            .AnyAsync(c => c.Name == dto.Name && c.Id != id))
            return (false, "Another course with this name already exists");

        if (dto.Name != null)
            course.Name = dto.Name;
        if (dto.Description != null)
            course.Description = dto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update course: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<(FacultyDto? Course, string? Error)>
        CreateFacultyAync(CreateFacultyDto dto)
    {
        if (await _context.Faculties.AnyAsync(c => c.Name == dto.Name))
            return (null, "Faculty with this name already exists");

        var faculty = new Faculty
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Faculties.Add(faculty);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create faculty: " +
                $"{ex.InnerException?.Message}");
        }

        return (new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        }, null);
    }

    public async Task<(bool Success, string? Error)>
        UpdateFacultyAsync(Guid id, UpdateFacultyDto dto)
    {
        var faculty = await _context.Faculties
            .FirstOrDefaultAsync(c => c.Id == id);

        if (faculty == null)
            return (false, "Faculty not found");

        if (dto.Name != null && await _context.Faculties
            .AnyAsync(c => c.Name == dto.Name && c.Id != id))
            return (false, "Another faculty with this name already exists");

        if (dto.Name != null)
            faculty.Name = dto.Name;
        if (dto.Description != null)
            faculty.Description = dto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to update faculty: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }

    public async Task<IEnumerable<FavoriteDto>> GetFavoritesAsync()
    {
        return await _context.Favorites
            .Select(f => new FavoriteDto
            {
                Id = f.Id,
                UserId = f.UserId,
                VacancyId = f.VacancyId,
                ResumeId = f.ResumeId,
                AddedAt = f.AddedAt
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)>
        DeleteFavoriteAsync(Guid id)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.Id == id);

        if (favorite == null)
            return (false, "Favorite not found");

        _context.Favorites.Remove(favorite);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete favorite: " +
                $"{ex.InnerException?.Message}");
        }

        return (true, null);
    }
}

