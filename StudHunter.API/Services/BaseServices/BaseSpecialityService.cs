using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.SpecialityDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseSpecialityService(StudHunterDbContext context) : BaseService(context)
{
    protected static SpecialityDto MapToSpecialityDto(Speciality speciality)
    {
        return new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        };
    }

    /// <summary>
    /// Retrieves all specialties.
    /// </summary>
    /// <returns>A tuple containing a list of all specialties, an optional status code, and an optional error message.</returns>
    public async Task<(List<SpecialityDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllSpecialtiesAsync()
    {
        var specialties = await _context.Specialities
            .Select(s => MapToSpecialityDto(s))
            .OrderByDescending(s => s.Name)
            .ToListAsync();

        return (specialties, null, null);
    }

    /// <summary>
    /// Retrieves a specialty by its ID.
    /// </summary>
    /// <param name="specialityId">The unique identifier (GUID) of the specialty.</param>
    /// <returns>A tuple containing the specialty, an optional status code, and an optional error message.</returns>
    public async Task<(SpecialityDto? Entity, int? StatusCode, string? ErrorMessage)> GetSpecialityAsync(Guid specialityId)
    {
        var speciality = await _context.Specialities.FindAsync(specialityId);
        if (speciality == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Speciality)));

        return (MapToSpecialityDto(speciality), null, null);
    }

    /// <summary>
    /// Retrieves a specialty by its name.
    /// </summary>
    /// <param name="specialityName"></param>
    /// <returns></returns>
    public async Task<(SpecialityDto? Entity, int? StatusCode, string? ErrorMessage)> GetSpecialityAsync(string specialityName)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Name == specialityName);
        if (speciality == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Speciality)));

        return (MapToSpecialityDto(speciality), null, null);
    }
}
