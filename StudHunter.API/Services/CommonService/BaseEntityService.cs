using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.CommonService;

public class BaseEntityService(StudHunterDbContext context)
{
    protected readonly StudHunterDbContext _context = context;

    protected async Task<(bool Success, string? Error)> DeleteEntityAsync<T>(Guid id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);

        if (entity == null)
            return (false, $"{typeof(T).Name} not found");

        _context.Set<T>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete {typeof(T).Name}: {ex.InnerException?.Message}");
        }
        return (true, null);
    }

    protected async Task<(bool Success, string? Error)> DeleteEntityAsync<T>(int id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);

        if (entity == null)
            return (false, $"{typeof(T).Name} not found");

        _context.Set<T>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete {typeof(T).Name}: {ex.InnerException?.Message}");
        }
        return (true, null);
    }
}
