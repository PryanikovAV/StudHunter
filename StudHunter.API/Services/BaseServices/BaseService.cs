using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.DB.Postgres;

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
}
