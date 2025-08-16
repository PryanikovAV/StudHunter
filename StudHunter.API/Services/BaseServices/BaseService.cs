using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Interfaces;

namespace StudHunter.API.Services.BaseServices;

/// <summary>
/// Base service with common database operations.
/// </summary>
public abstract class BaseService(StudHunterDbContext context)
{
    protected readonly StudHunterDbContext _context = context;

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>A tuple indicating whether the save was successful, an optional status code, and an optional error message.</returns>
    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> SaveChangesAsync<T>()
    {
        try
        {
            await _context.SaveChangesAsync();
            return (true, null, null);
        }
        catch (DbUpdateException)
        {
            return (false, StatusCodes.Status400BadRequest, ErrorMessages.InvalidData(typeof(T).Name));
        }
    }

    /// <summary>
    /// Deletes an entity, using soft delete if supported, otherwise hard delete.
    /// </summary>
    /// <typeparam name="T">The type of the entity, implementing IEntity.</typeparam>
    /// <param name="id">The unique identifier (GUID) of the entity.</param>
    /// <param name="hardDelete">A boolean indicating whether to perform a hard delete (true) or soft delete (false).</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEntityAsync<T>(Guid id, bool hardDelete = false) where T : class, IEntity
    {
        var entity = await _context.Set<T>().FindAsync(id);

        if (entity == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(typeof(T).Name));

        if (hardDelete)
        {
            _context.Set<T>().Remove(entity);
            return await SaveChangesAsync<T>();
        }

        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
        {
            var softDelitable = (ISoftDeletable)entity;

            if (softDelitable.IsDeleted)
                return (false, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(typeof(T).Name));

            softDelitable.IsDeleted = true;
            return await SaveChangesAsync<T>();
        }

        return (false, StatusCodes.Status400BadRequest, $"Soft delete is not supported for {typeof(T).Name.ToLower()}");
    }
}
