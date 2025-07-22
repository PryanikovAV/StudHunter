using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.CommonService;

public class BaseService(StudHunterDbContext context)
{
    protected readonly StudHunterDbContext _context = context;

    /// <summary>
    /// Saves changes to the database and handles potential errors
    /// </summary>
    /// <param name="action">The action being performed (e.g. 'create', 'update').</param>
    /// <param name="entityName">The name of the entity (e.g. 'Student', 'Course').</param>
    /// <returns>A tuple indicating success and an optional error message.</returns>
    protected async Task<(bool Success, string? Error)> SaveChangesAsync(string action, string entityName)
    {
        try
        {
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to {action} {entityName.ToLower()}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    /// <summary>
    /// Performs a soft delete on an entity by setting the IsDeleted flag to true. Only applies to entities that inherit from User.
    /// </summary>
    /// <typeparam name="T">Entity type, must inherit from User.</typeparam>
    /// <param name="id">Unique entity identifier (Guid).</param>
    /// <returns>A tuple containing the result of the operation and an error message, if any.</returns>
    protected async Task<(bool Success, string? Error)> SoftDeleteEntityAsync<T>(Guid id) where T : User
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
            return (false, $"{typeof(T).Name} not found");

        entity.IsDeleted = true;

        return await SaveChangesAsync("soft delete", $"{typeof(T).Name}");
    }

    /// <summary>
    /// Performs a hard delete of an entity from the database.
    /// </summary>
    /// <param name="id">Unique entity identifier (Guid).</param>
    /// <returns>A tuple containing the result and an error message, if any.</returns>
    protected async Task<(bool Success, string? Error)> HardDeleteEntityAsync<T>(Guid id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
            return (false, $"{typeof(T).Name} not found");

        _context.Set<T>().Remove(entity);

        return await SaveChangesAsync("hard delete", $"{typeof(T).Name}");
    }

    /// <summary>
    /// Performs a hard delete of an entity from the database.
    /// </summary>
    /// <param name="id">Unique entity identifier (int).</param>
    /// <returns>A tuple containing the result and an error message, if any.</returns>
    protected async Task<(bool Success, string? Error)> HardDeleteEntityAsync<T>(int id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
            return (false, $"{typeof(T).Name} not found");

        _context.Set<T>().Remove(entity);

        return await SaveChangesAsync("hard delete", $"{typeof(T).Name}");
    }
}
