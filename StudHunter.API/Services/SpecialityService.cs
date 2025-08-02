using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing specialties.
/// </summary>
public class SpecialityService(StudHunterDbContext context) : BaseService(context)
{
    /// <summary>
    /// Retrieves all specialties.
    /// </summary>
    /// <returns>A tuple containing a list of all specialties, an optional status code, and an optional error message.</returns>
    public async Task<(List<SpecialityDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllSpecialtiesAsync()
    {
        var specialties = await _context.Specialities
        .Select(s => new SpecialityDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description
        })
        .ToListAsync();

        return (specialties, null, null);
    }

    /// <summary>
    /// Retrieves a specialty by its ID.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the specialty.</param>
    /// <returns>A tuple containing the specialty, an optional status code, and an optional error message.</returns>
    public async Task<(SpecialityDto? Entity, int? StatusCode, string? ErrorMessage)> GetSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities.FindAsync(id);

        #region Serializers
        if (speciality == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Speciality)));
        #endregion

        return (new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        }, null, null);
    }
}
