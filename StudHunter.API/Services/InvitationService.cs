using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing invitations.
/// </summary>
public class InvitationService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    private readonly UserAchievementService _userAchievementService = userAchievementService;

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
    /// Retrieves an invitation by its ID for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <param name="userId">The unique identifier (GUID) of the user (sender).</param>
    /// <returns>A tuple containing the invitation, an optional status code, and an optional error message.</returns>
    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> GetInvitationAsync(Guid id, Guid userId)
    {
        var invitation = await _context.Invitations
        .Where(i => i.Id == id && i.SenderId == userId)
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
        .FirstOrDefaultAsync();

        #region Serializers
        if (invitation == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Invitation)));
        #endregion

        return (invitation, null, null);
    }

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    /// <param name="senderId">The unique identifier (GUID) of the sender.</param>
    /// <param name="dto">The data transfer object containing invitation details.</param>
    /// <returns>A tuple containing the created invitation, an optional status code, and an optional error message.</returns>
    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> CreateInvitationAsync(Guid senderId, CreateInvitationDto dto)
    {
        #region Serializers
        if (senderId == dto.ReceiverId)
            return (null, StatusCodes.Status400BadRequest, "Sender and receiver cannot be the same.");

        if (dto.VacancyId == null && dto.ResumeId == null)
            return (null, StatusCodes.Status400BadRequest, "Either vacancyId or resumeId must be provided.");

        if (dto.VacancyId != null && dto.ResumeId != null)
            return (null, StatusCodes.Status400BadRequest, "Only one of vacancyId or resumeId can be provided.");

        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("sender"));

        var receiver = await _context.Users.FindAsync(dto.ReceiverId);
        if (receiver == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("receiver"));

        if (dto.VacancyId != null && !await _context.Vacancies.AnyAsync(v => v.Id == dto.VacancyId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Vacancy)));

        if (dto.ResumeId != null && !await _context.Resumes.AnyAsync(r => r.Id == dto.ResumeId))
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Resume)));

        if (dto.Type == nameof(Invitation.Type.EmployerToStudent))
        {
            if (sender is not Employer || receiver is not Student)
                return (null, StatusCodes.Status400BadRequest, $"{nameof(Invitation.Type.EmployerToStudent)} {nameof(Invitation)} must be sent by an {nameof(Employer)} to a {nameof(Student)}.");
            if (dto.VacancyId == null)
                return (null, StatusCodes.Status400BadRequest, $"VacancyId is required for {nameof(Invitation.Type.EmployerToStudent)} {nameof(Invitation)}.");
        }
        else if (dto.Type == nameof(Invitation.Type.StudentToEmployer))
        {
            if (sender is not Student || receiver is not Employer)
                return (null, StatusCodes.Status400BadRequest, $"{nameof(Invitation.Type.StudentToEmployer)} {nameof(Invitation)} must be sent by a {nameof(Student)} to an {nameof(Employer)}.");
            if (dto.ResumeId == null)
                return (null, StatusCodes.Status400BadRequest, $"ResumeId is required for {nameof(Invitation.Type.StudentToEmployer)} {nameof(Invitation)}.");
        }

        if (await _context.Invitations.AnyAsync(i => i.SenderId == senderId && i.ReceiverId == dto.ReceiverId && (i.VacancyId == dto.VacancyId || i.ResumeId == dto.ResumeId)))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists(nameof(Invitation), "senderId, receiverId, vacancyId/resumeId"));
        #endregion

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            Type = Enum.Parse<Invitation.InvitationType>(dto.Type),
            Message = dto.Message,
            Status = Invitation.InvitationStatus.Sent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Invitations.Add(invitation);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Invitation>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (new InvitationDto
        {
            Id = invitation.Id,
            SenderId = invitation.SenderId,
            ReceiverId = invitation.ReceiverId,
            VacancyId = invitation.VacancyId,
            ResumeId = invitation.ResumeId,
            Type = invitation.Type.ToString(),
            Message = invitation.Message,
            Status = invitation.Status.ToString(),
            CreatedAt = invitation.CreatedAt,
            UpdatedAt = invitation.UpdatedAt,
            VacancyStatus = invitation.VacancyId.HasValue
        ? (await _context.Vacancies.AnyAsync(v => v.Id == invitation.VacancyId && !v.IsDeleted) ? "Active" : "Deleted") : null,
            ResumeStatus = invitation.ResumeId.HasValue
        ? (await _context.Resumes.AnyAsync(r => r.Id == invitation.ResumeId && !r.IsDeleted) ? "Active" : "Deleted") : null
        }, null, null);
    }

    /// <summary>
    /// Updates the status of an existing invitation.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <param name="receiverId">The unique identifier (GUID) of the receiver.</param>
    /// <param name="dto">The data transfer object containing the updated invitation status.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateInvitationStatusAsync(Guid id, Guid receiverId, UpdateInvitationDto dto)
    {
        var invitation = await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id && i.ReceiverId == receiverId);

        #region Serializers
        if (invitation == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound(nameof(Invitation)));

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return (false, StatusCodes.Status409Conflict, $"{nameof(Invitation)} status cannot be changed.");
        #endregion

        invitation.Status = Enum.Parse<Invitation.InvitationStatus>(dto.Status);
        invitation.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Invitation>();

        return (success, statusCode, errorMessage);
    }
}
