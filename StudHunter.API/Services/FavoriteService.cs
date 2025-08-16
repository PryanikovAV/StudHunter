using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing favorites.
/// </summary>
public class FavoriteService(StudHunterDbContext context) : BaseFavoriteService(context)
{
    /// <summary>
    /// Creates a new favorite for a user.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="dto">The data transfer object containing favorite details.</param>
    /// <returns>A tuple containing the created favorite, an optional status code, and an optional error message.</returns>
    public async Task<(FavoriteDto? Entity, int? StatusCode, string? ErrorMessage)> CreateFavoriteAsync(Guid userId, CreateFavoriteDto dto)
    {
        if (dto.VacancyId == null && dto.EmployerId == null && dto.StudentId == null)
            return (null, StatusCodes.Status400BadRequest, "At least one of vacancyId, employerId, or studentId must be provided.");

        if ((dto.VacancyId == null && (dto.EmployerId != null || dto.StudentId != null)) ||
            (dto.EmployerId != null && (dto.VacancyId != null || dto.StudentId != null)) ||
            (dto.StudentId != null && (dto.VacancyId != null || dto.EmployerId != null)))
            return (null, StatusCodes.Status400BadRequest, "Only one of vacancyId, employerId, or studentId can be provided.");

        if (dto.VacancyId != null && !await _context.Vacancies.AnyAsync(c => c.Id == dto.VacancyId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        if (dto.EmployerId != null && !await _context.Employers.AnyAsync(e => e.Id == dto.EmployerId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        if (dto.StudentId != null && !await _context.Students.AnyAsync(s => s.Id == dto.StudentId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));

        if (await _context.Favorites.AnyAsync(f => f.UserId == userId &&
            (f.VacancyId == dto.VacancyId || f.EmployerId == dto.EmployerId || f.StudentId == dto.StudentId)))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Favorite), "userId, favoriteId"));

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(User)));

        if (user is Student && dto.StudentId != null)
            return (null, StatusCodes.Status400BadRequest, "Students can only favorite vacancies or employers.");

        if (user is Employer && (dto.VacancyId != null || dto.EmployerId != null))
            return (null, StatusCodes.Status400BadRequest, "Employers can only favorire students.");

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VacancyId = dto.VacancyId,
            EmployerId = dto.EmployerId,
            StudentId = dto.StudentId,
            AddedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Favorite>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToFavoriteDto(favorite), null, null);
    }

    /// <summary>
    /// Deletes a favorite for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the favorite.</param>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteFavoriteAsync(Guid id, Guid userId)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Favorite)));

        if (favorite.UserId != userId)
            return (false, StatusCodes.Status403Forbidden, $"Cannot delete {nameof(Favorite)} belonging to another {nameof(User)}.");

        return await DeleteEntityAsync<Favorite>(id, hardDelete: true);
    }
}
