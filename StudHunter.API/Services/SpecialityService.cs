using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public class SpecialityService(StudHunterDbContext context) : BaseEntityService(context)
{
    public async Task<IEnumerable<SpecialityDto>> GetAllSpecialitiesAsync()
    {
        return await _context.Specialities.Select(s => new SpecialityDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description
        })
            .ToListAsync();
    }

    public async Task<SpecialityDto?> GetSpecialityAsync(Guid id)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);

        if (speciality == null)
            return null;

        return new SpecialityDto
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description
        };
    }
}
