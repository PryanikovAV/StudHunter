using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing invitations with administrative privileges.
/// </summary>
public class AdminInvitationService(StudHunterDbContext context) : BaseInvitationService(context)
{
    /// <summary>
    /// Retrieves all invitations.
    /// </summary>
    /// <returns>A tuple containing a list of all invitations, an optional status code, and an optional error message.</returns>
    public async Task<(List<InvitationDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllInvitationsAsync()
    {
        var invitations = await _context.Invitations
        .Include(i => i.Vacancy)
        .Include(i => i.Resume)
        .Select(i => MapToInvitationDto(i))
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync();

        return (invitations, null, null);
    }

    /// <summary>
    /// Deletes an invitation.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <returns>A tuple indicating whether the deletion was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteInvitationAsync(Guid id)
    {
        return await DeleteEntityAsync<Invitation>(id, hardDelete: true);
    }
}
