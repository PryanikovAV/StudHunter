﻿using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Favorite;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class FavoriteService(StudHunterDbContext context)
{
    private readonly StudHunterDbContext _context = context;

    public async Task<IEnumerable<FavoriteDto>>
        GetFavoritesAsync(Guid userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => new FavoriteDto
            {
                Id = f.Id,
                UserId = f.UserId,
                VacancyId = f.VacancyId,
                ResumeId = f.ResumeId,
                AddedAt = f.AddedAt
            })
            .ToListAsync();
    }

    public async Task<(FavoriteDto? Favorite, string? Error)>
        CreateFavoriteAsync(Guid userId, CreateFavoriteDto dto)
    {
        if (dto.VacancyId == null && dto.ResumeId == null)
            return (null, "Either VacancyId or ResumeId must be provided.");

        if (dto.VacancyId != null && dto.ResumeId != null)
            return (null, "Only one of VacancyId or ResumeId can be provided.");

        if (dto.VacancyId != null && !await _context.Vacancies.AnyAsync(v => v.Id == dto.VacancyId)
            return (null, "Vacancy not found.");

        if (dto.ResumeId != null && !await _context.Resumes.AnyAsync(r => r.Id == dto.ResumeId))
            return (null, "Resume not found.");

        if (await _context.Favorites.AnyAsync(f => f.UserId == userId &&
            (f.VacancyId == dto.VacancyId || f.ResumeId == dto.ResumeId)))
            return (null, "Favorite already exists.");

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            AddedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (null, $"Failed to create favorite: {ex.InnerException?.Message}");
        }

        return (new FavoriteDto
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            VacancyId = favorite.VacancyId,
            ResumeId = favorite.ResumeId,
            AddedAt = favorite.AddedAt
        }, null);
    }

    public async Task<(bool Success, string? Error)> DeleteFavoriteAsync(Guid id)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.Id == id);

        if (favorite == null)
            return (false, "Favorite not found");

        _context.Favorites.Remove(favorite);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return (false, $"Failed to delete favorite: {ex.InnerException?.Message}");
        }

        return (true, null);
    }   
}