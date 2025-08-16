using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseInvitationService(StudHunterDbContext context) : BaseService(context)
{
    protected static InvitationDto MapToInvitationDto(Invitation invitation)
    {
        return new InvitationDto
        {
            Id = invitation.Id,
            SenderId = invitation.SenderId,
            ReceiverId = invitation.ReceiverId,
            VacancyId = invitation.VacancyId,
            ResumeId = invitation.ResumeId,
            Type = invitation.Type.ToString(),
            Status = invitation.Status.ToString(),
            CreatedAt = invitation.CreatedAt,
            UpdatedAt = invitation.UpdatedAt,
            VacancyStatus = invitation.Vacancy != null ? (invitation.Vacancy.IsDeleted ? "Deleted" : "Active") : null,
            ResumeStatus = invitation.Resume != null ? (invitation.Resume.IsDeleted ? "Deleted" : "Active") : null
        };
    }

    /// <summary>
    /// Retrieves invitations for a specific user, either sent or received.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="sent">A boolean indicating whether to retrieve sent (true) or received (false) invitations.</param>
    /// <returns>A tuple containing a list of invitations, an optional status code, and an optional error message.</returns>
    public async Task<(List<InvitationDto>? Entities, int? StatusCode, string? ErrorMessage)> GetInvitationsByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Invitations.Where(i => i.SenderId == userId)
        : _context.Invitations.Where(i => i.ReceiverId == userId);

        var invitations = await query
        .Include(i => i.Vacancy)
        .Include(i => i.Resume)
        .Select(i => MapToInvitationDto(i))
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync();

        return (invitations, null, null);
    }

    /// <summary>
    /// Retrieves an invitation by its ID for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <param name="userId">The unique identifier (GUID) of the user (sender).</param>
    /// <returns>A tuple containing the invitation, an optional status code, and an optional error message.</returns>
    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> GetInvitationAsync(Guid id, Guid userId)
    {
        var invitation = await _context.Invitations
        .Include(i => i.Vacancy)
        .Include(i => i.Resume)
        .FirstOrDefaultAsync(i => i.Id == id && (i.SenderId == userId || i.ReceiverId == userId));

        if (invitation == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Invitation)));

        return (MapToInvitationDto(invitation), null, null);
    }
}
