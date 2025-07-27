using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public class SpecialityService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<(List<SpecialityDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllSpecialtiesAsync()
    {
        var specialties = await _context.Specialities.Select(s => new SpecialityDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description
        }).ToListAsync();

        return (specialties, null, null);
    }

    public async Task<(SpecialityDto? Entity, int? StatusCode, string? ErrorMessage)> GetSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);

        #region Serializers
        if (speciality == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Speciality"));
        #endregion

        return (new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        }, null, null);
    }
}
