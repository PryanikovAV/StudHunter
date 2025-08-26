using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
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
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> GetInvitationAsync(Guid id)
    {
        var invitation = await _context.Invitations
        .Include(i => i.Sender)
        .Include(i => i.Receiver)
        .Include(i => i.Vacancy)
        .Include(i => i.Resume)
        .FirstOrDefaultAsync(i => i.Id == id);
        if (invitation == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Invitation)));
        return (MapToInvitationDto(invitation), null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="sent"></param>
    /// <returns></returns>
    public async Task<(List<InvitationDto>? Entities, int? StatusCode, string? ErrorMessage)> GetInvitationsByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Invitations.Where(i => i.SenderId == userId)
        : _context.Invitations.Where(i => i.ReceiverId == userId);

        var invitations = await query
        .Include(i => i.Sender)
        .Include(i => i.Receiver)
        .Include(i => i.Vacancy)
        .Include(i => i.Resume)
        .Select(i => MapToInvitationDto(i))
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync();
        return (invitations, null, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateInvitationStatusAsync(Guid id, string status)
    {
        var invitation = await _context.Invitations
        .FirstOrDefaultAsync(i => i.Id == id);
        if (invitation == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Invitation)));
        if (!Enum.TryParse<Invitation.InvitationStatus>(status, out var newStatus))
            return (false, StatusCodes.Status400BadRequest, "Invalid status value.");
        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return (false, StatusCodes.Status409Conflict, $"{nameof(Invitation)} status cannot be changed.");

        invitation.Status = newStatus;
        invitation.UpdatedAt = DateTime.UtcNow;
        return await SaveChangesAsync<Invitation>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteInvitationAsync(Guid id)
    {
        var invitation = await _context.Invitations.FindAsync(id);
        if (invitation == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Invitation)));
        _context.Invitations.Remove(invitation);
        return await SaveChangesAsync<Invitation>();
    }
}
