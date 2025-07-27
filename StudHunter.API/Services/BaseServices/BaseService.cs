using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Interfaces;

namespace StudHunter.API.Services.BaseServices;

public class BaseService(StudHunterDbContext context)
{
    protected readonly StudHunterDbContext _context = context;

    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> SaveChangesAsync(string entityName)
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

    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> SoftDeleteEntityAsync<T>(Guid id) where T : class, ISoftDeletable
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        #region Serializers
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(typeof(T).Name));

        if (entity.IsDeleted)
            return (false, StatusCodes.Status410Gone, ErrorMessages.AlreadyDeleted(typeof(T).Name));
        #endregion

        entity.IsDeleted = true;

        var (success, statusCode, errorMessage) = await SaveChangesAsync(typeof(T).Name);
        return (success, statusCode, errorMessage);
    }

    protected async Task<(bool Success, int? StatusCode, string? ErrorMessage)> HardDeleteEntityAsync<T>(Guid id) where T : class, IEntity
    {
        var entity = await _context.Set<T>().FindAsync(id);

        #region Serializers
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(typeof(T).Name));
        #endregion

        _context.Set<T>().Remove(entity);

        var (success, statusCode, errorMessage) = await SaveChangesAsync(typeof(T).Name);
        return (success, statusCode, errorMessage);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteEntityAsync<T>(Guid id, bool hardDelete = false) where T : class, IEntity
    {
        if (hardDelete)
            return await HardDeleteEntityAsync<T>(id);

        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);

            #region Serializers
            if (entity == null)
                return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(typeof(T).Name));
            #endregion

            if (entity is ISoftDeletable softDeletable)
            {
                #region Serializers
                if (softDeletable.IsDeleted)
                    return (false, StatusCodes.Status410Gone, ErrorMessages.AlreadyDeleted(typeof(T).Name));
                #endregion

                softDeletable.IsDeleted = true;
                var (success, statusCode, errorMessage) = await SaveChangesAsync(typeof(T).Name);

                return (success, statusCode, errorMessage);

            }
        }
        
        return await HardDeleteEntityAsync<T>(id);
    }
}
