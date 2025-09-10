using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.FavoriteDto;
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
    /// Retrieves all favorites for a specific user.
    /// </summary>
    /// <param name="authUserId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing a list of favorites, an optional status code, and an optional error message.</returns>
    public async Task<(List<FavoriteDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllFavoritesByUserAsync(Guid authUserId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == authUserId))
            return (null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var favorites = await _context.Favorites
            .Include(f => f.Vacancy)
            .Include(f => f.Employer)
            .Include(f => f.Student)
            .Where(f => f.UserId == authUserId)
            .Select(f => MapToFavoriteDto(f))
            .ToListAsync();

        return (favorites, null, null);
    }

    /// <summary>
    /// Retrieves a favorite by its ID for a specific user.
    /// </summary>
    /// <param name="favoriteId">The unique identifier (GUID) of the favorite.</param>
    /// <param name="authUserId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple containing the favorite, an optional status code, and an optional error message.</returns>
    public async Task<(FavoriteDto? Entity, int? StatusCode, string? ErrorMessage)> GetFavoriteAsync(Guid authUserId, Guid favoriteId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == authUserId))
            return (null, StatusCodes.Status403Forbidden, ErrorMessages.InvalidTokenUserId());

        var favorite = await _context.Favorites
            .Include(f => f.Vacancy)
            .Include(f => f.Employer)
            .Include(f => f.Student)
            .Where(f => f.Id == favoriteId && f.UserId == authUserId)
            .Select(f => MapToFavoriteDto(f))
            .FirstOrDefaultAsync();

        if (favorite == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Favorite)));

        return (favorite, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authUserId"></param>
    /// <returns></returns>
    public async Task<(List<FavoriteDto>? Entities, int? StatusCode, string? ErrorMessage)> GetFavoriteVacanciesAsync(Guid authUserId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == authUserId))
            return (null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var isStudent = await _context.Students.AnyAsync(s => s.Id == authUserId && !s.IsDeleted);
        if (!isStudent)
            return (null, StatusCodes.Status403Forbidden, $"Only {nameof(Student)} can favorite {nameof(Vacancy)}.");

        var favorites = await _context.Favorites
            .Include(f => f.Vacancy)
            .Where(f => f.UserId == authUserId && f.VacancyId != null)
            .Select(f => MapToFavoriteDto(f))
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync();

        return (favorites, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authUserId"></param>
    /// <returns></returns>
    public async Task<(List<FavoriteDto>? Entities, int? StatusCode, string? ErrorMessage)> GetFavoriteEmployersAsync(Guid authUserId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == authUserId))
            return (null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var isStudent = await _context.Students.AnyAsync(s => s.Id == authUserId && !s.IsDeleted);
        if (!isStudent)
            return (null, StatusCodes.Status403Forbidden, $"Only {nameof(Student)} can favorite {nameof(Employer)}.");

        var favorites = await _context.Favorites
            .Include(f => f.Employer)
            .Where(f => f.UserId == authUserId && f.EmployerId != null)
            .Select(f => MapToFavoriteDto(f))
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync();

        return (favorites, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authUserId"></param>
    /// <returns></returns>
    public async Task<(List<FavoriteDto>? Entities, int? StatusCode, string? ErrorMessage)> GetFavoriteStudentsAsync(Guid authUserId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == authUserId))
            return (null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var isEmployer = await _context.Employers.AnyAsync(s => s.Id == authUserId && !s.IsDeleted);
        if (!isEmployer)
            return (null, StatusCodes.Status403Forbidden, $"Only {nameof(Employer)} can favorite {nameof(Student)}.");

        var favorites = await _context.Favorites
            .Include(f => f.Student)
            .Where(f => f.UserId == authUserId && f.StudentId != null)
            .Select(f => MapToFavoriteDto(f))
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync();

        return (favorites, null, null);
    }

    public async Task<(bool Exists, int? StatusCode, string? ErrorMessage)> IsFavoriteAsync(Guid authUserId, Guid? vacancyId, Guid? employerId, Guid? studentId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == authUserId))
            return (false, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var exists = await _context.Favorites
            .AnyAsync(f => f.UserId == authUserId && (f.VacancyId == vacancyId || f.EmployerId == employerId || f.StudentId == studentId));

        return (exists, null, null);
    }

    /// <summary>
    /// Creates a new favorite for a user.
    /// </summary>
    /// <param name="authUserId">The unique identifier (GUID) of the user.</param>
    /// <param name="dto">The data transfer object containing favorite details.</param>
    /// <returns>A tuple containing the created favorite, an optional status code, and an optional error message.</returns>
    public async Task<(FavoriteDto? Entity, int? StatusCode, string? ErrorMessage)> CreateFavoriteAsync(Guid authUserId, CreateFavoriteDto dto)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == authUserId))
            return (null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        if (dto.VacancyId != null && !await _context.Vacancies.AnyAsync(v => v.Id == dto.VacancyId && !v.IsDeleted))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));

        if (dto.EmployerId != null && !await _context.Employers.AnyAsync(e => e.Id == dto.EmployerId && !e.IsDeleted && e.AccreditationStatus))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Employer)));

        if (dto.StudentId != null && !await _context.Students.AnyAsync(s => s.Id == dto.StudentId && !s.IsDeleted && s.Resume != null && !s.Resume.IsDeleted))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Student)));

        if (await _context.Favorites.AnyAsync(f => f.UserId == authUserId && (f.VacancyId == dto.VacancyId || f.EmployerId == dto.EmployerId || f.StudentId == dto.StudentId)))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Favorite), "userId, favoriteId"));

        var isStudent = await _context.Students.AnyAsync(s => s.Id == authUserId && !s.IsDeleted);
        var isEmployer = await _context.Employers.AnyAsync(e => e.Id == authUserId && !e.IsDeleted && e.AccreditationStatus);

        if (isStudent && dto.StudentId != null)
            return (null, StatusCodes.Status400BadRequest, $"{nameof(Student)} can only favorite {nameof(Vacancy)} or {nameof(Employer)}.");
        if (isEmployer && (dto.VacancyId != null || dto.EmployerId != null))
            return (null, StatusCodes.Status400BadRequest, $"{nameof(Employer)} can only favorite {nameof(Student)}.");

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = authUserId,
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
    /// <param name="favoriteId">The unique identifier (GUID) of the favorite.</param>
    /// <param name="authUserId">The unique identifier (GUID) of the user.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public virtual async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteFavoriteAsync(Guid authUserId, Guid favoriteId)
    {
        var favorite = await _context.Favorites.FindAsync(favoriteId);
        if (favorite == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Favorite)));
        if (favorite.UserId != authUserId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("delete", nameof(Favorite)));

        _context.Favorites.Remove(favorite);

        return await SaveChangesAsync<Favorite>();
    }
}
