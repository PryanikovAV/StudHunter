using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Faculty;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminFacultyService(StudHunterDbContext context) : FacultyService(context)
{
    public async Task<(FacultyDto? Entity, int? StatusCode, string? ErrorMessage)> CreateFacultyAsync(CreateFacultyDto dto)
    {
        #region Serializers
        var facultyExists = await _context.Faculties.AnyAsync(f => f.Name == dto.Name);
        if (facultyExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Faculty", "Name"));
        #endregion

        var faculty = new Faculty
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Faculties.Add(faculty);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Faculty>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            Description = faculty.Description
        }, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateFacultyAsync(Guid id, UpdateFacultyDto dto)
    {
        var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == id);

        #region Serializers
        if (faculty == null)
            return (false, StatusCodes.Status404NotFound, "Faculty");

        var facultyExists = await _context.Faculties.AnyAsync(f => f.Name == dto.Name && f.Id != id);
        if (facultyExists)
            return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Faculty", "Name"));
        #endregion

        if (dto.Name != null)
            faculty.Name = dto.Name;
        if (dto.Description != null)
            faculty.Description = dto.Description;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Faculty>();

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteFacultyAsync(Guid id)
    {
        #region Serializers
        var facultyAssociatedStudyPlan = await _context.StudyPlans.AnyAsync(sp => sp.FacultyId == id);
        if (facultyAssociatedStudyPlan)
            return (false, StatusCodes.Status400BadRequest, "Cannot delete faculty associated with study plans");
        #endregion

        return await DeleteEntityAsync<Faculty>(id, hardDelete: true);
    }
}
