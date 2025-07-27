using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class AdminSpecialityService(StudHunterDbContext context) : SpecialityService(context)
{
    public async Task<(SpecialityDto? Entity, int? StatusCode, string? ErrorMessage)> CreateSpecialityAsync(CreateSpecialityDto dto)
    {
        #region Serializers
        var specialityExists = await _context.Specialities.AnyAsync(s => s.Name == dto.Name);
        if (specialityExists)
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Speciality", "Name"));
        #endregion

        var speciality = new Speciality
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Specialities.Add(speciality);

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Speciality");

        if (!success)
            return (null, statusCode, errorMessage);

        return (new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        }, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateSpecialityAsync(Guid id, UpdateSpecialityDto dto)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);

        #region Serializers
        if (speciality == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Speciality"));

        var specialityExists = await _context.Specialities.AnyAsync(s => s.Name == dto.Name && s.Id != id);
        if (specialityExists)
            return (false, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Speciality", "Name"));
        #endregion

        if (dto.Name != null)
            speciality.Name = dto.Name;
        if (dto.Description != null)
            speciality.Description = dto.Description;

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Speciality");

        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);

        #region Serializers
        if (speciality == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Speciality"));

        if (await _context.StudyPlans.AnyAsync(sp => sp.SpecialityId == id))
            return (false, StatusCodes.Status400BadRequest, "Cannot delete speciality associated with study plans");
        #endregion

        return await DeleteEntityAsync<Speciality>(id, hardDelete: true);
    }
}
