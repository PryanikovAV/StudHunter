using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

/// <summary>
/// Service for managing invitations with administrative privileges.
/// </summary>
public class AdminInvitationService(StudHunterDbContext context, UserAchievementService userAchievementService)
: InvitationService(context, userAchievementService)
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
        .Select(i => new InvitationDto
        {
            Id = i.Id,
            SenderId = i.SenderId,
            ReceiverId = i.ReceiverId,
            VacancyId = i.VacancyId,
            ResumeId = i.ResumeId,
            Type = i.Type.ToString(),
            Message = i.Message,
            Status = i.Status.ToString(),
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            VacancyStatus = i.Vacancy != null ? (i.Vacancy.IsDeleted ? "Deleted" : "Active") : null,
            ResumeStatus = i.Resume != null ? (i.Resume.IsDeleted ? "Deleted" : "Active") : null
        })
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
