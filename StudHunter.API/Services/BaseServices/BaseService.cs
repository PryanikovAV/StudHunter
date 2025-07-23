using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Interfaces;

namespace StudHunter.API.Services.BaseServices;

public class BaseService(StudHunterDbContext context)
{
    protected readonly StudHunterDbContext _context = context;

    /// <summary>
    /// Saves changes to the database and handles potential errors.
    /// </summary>
    /// <param name="action">The action being performed (e.g., 'create', 'update').</param>
    /// <param name="entityName">The name of the entity (e.g., 'Student', 'Course').</param>
    /// <returns>A tuple indicating success, HTTP status code, and an optional error message.</returns>
    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> SaveChangesAsync(string action, string entityName)
    {
        try
        {
            await _context.SaveChangesAsync();
            return (true, null, null);
        }
        catch (DbUpdateException)
        {
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.InvalidData(entityName));
        }
    }

    /// <summary>
    /// Performs a soft delete on an entity by setting the IsDeleted flag to true.
    /// </summary>
    /// <typeparam name="T">Entity type implementing ISoftDeletable.</typeparam>
    /// <param name="id">Unique entity identifier (Guid).</param>
    /// <returns>A tuple containing the result of the operation, HTTP status code, and an optional error message.</returns>
    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> SoftDeleteEntityAsync<T>(Guid id) where T : class, ISoftDeletable
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(typeof(T).Name));

        entity.IsDeleted = true;
        var (success, statusCode, errorMessage) = await SaveChangesAsync("soft delete", typeof(T).Name);
        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Performs a hard delete of an entity from the database.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="id">Unique entity identifier (Guid).</param>
    /// <returns>A tuple containing the result of the operation, HTTP status code, and an optional error message.</returns>
    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> HardDeleteEntityAsync<T>(Guid id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(typeof(T).Name));

        _context.Set<T>().Remove(entity);
        var (success, statusCode, errorMessage) = await SaveChangesAsync("hard delete", typeof(T).Name);
        return (success, statusCode, errorMessage);
    }

    /// <summary>
    /// Performs a hard delete of an entity from the database.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="id">Unique entity identifier (int).</param>
    /// <returns>A tuple containing the result of the operation, HTTP status code, and an optional error message.</returns>
    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> HardDeleteEntityAsync<T>(int id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(typeof(T).Name));

        _context.Set<T>().Remove(entity);
        var (success, statusCode, errorMessage) = await SaveChangesAsync("hard delete", typeof(T).Name);
        return (success, statusCode, errorMessage);
    }
}
