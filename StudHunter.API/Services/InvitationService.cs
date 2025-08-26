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
public class InvitationService(StudHunterDbContext context) : BaseInvitationService(context)
{
    /// <summary>
    /// Retrieves invitations for a specific user, either sent or received.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="sent">A boolean indicating whether to retrieve sent (true) or received (false) invitations.</param>
    /// <returns>A tuple containing a list of invitations, an optional status code, and an optional error message.</returns>
    public async Task<(List<InvitationDto>? Entities, int? StatusCode, string? ErrorMessage)> GetInvitationsByUserAsync(Guid userId, bool sent = false)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return (null, StatusCodes.Status400BadRequest, ErrorMessages.EntityNotFound(nameof(User)));

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
    /// Retrieves an invitation by its ID for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <param name="userId">The unique identifier (GUID) of the user (sender).</param>
    /// <returns>A tuple containing the invitation, an optional status code, and an optional error message.</returns>
    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> GetInvitationAsync(Guid userId, Guid id)
    {
        var invitation = await _context.Invitations
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Vacancy)
            .Include(i => i.Resume)
            .FirstOrDefaultAsync(i => i.Id == id && (i.SenderId == userId || i.ReceiverId == userId));

        if (invitation == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Invitation)));

        return (MapToInvitationDto(invitation), null, null);
    }

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    /// <param name="senderId">The unique identifier (GUID) of the sender.</param>
    /// <param name="dto">The data transfer object containing invitation details.</param>
    /// <returns>A tuple containing the created invitation, an optional status code, and an optional error message.</returns>
    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> CreateInvitationAsync(Guid senderId, CreateInvitationDto dto)
    {
        if (senderId == dto.ReceiverId)
            return (null, StatusCodes.Status400BadRequest, "Sender and receiver cannot be the same.");

        if (dto.VacancyId == null && dto.ResumeId == null)
            return (null, StatusCodes.Status400BadRequest, $"Either {nameof(Vacancy)}Id or {nameof(Resume)}Id must be provided.");

        if (dto.VacancyId != null && dto.ResumeId != null)
            return (null, StatusCodes.Status400BadRequest, $"Only one of {nameof(Vacancy)}Id or {nameof(Resume)}Id can be provided.");

        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("sender"));

        var receiver = await _context.Users.FindAsync(dto.ReceiverId);
        if (receiver == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("receiver"));

        if (dto.VacancyId != null)
        {
            var vacancy = await _context.Vacancies.FindAsync(dto.VacancyId);
            if (vacancy == null)
                return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));
            if (vacancy.EmployerId != senderId)
                return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("invite using", nameof(Vacancy)));
        }

        if (dto.ResumeId != null)
        {
            var resume = await _context.Resumes.FindAsync(dto.ResumeId);
            if (resume == null)
                return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
            if (resume.StudentId != senderId)
                return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("respond using", nameof(Resume)));
        }

        if (dto.Type == nameof(Invitation.Type.EmployerToStudent))
        {
            if (sender is not Employer || receiver is not Student)
                return (null, StatusCodes.Status400BadRequest, $"{nameof(Invitation.Type.EmployerToStudent)} {nameof(Invitation)} must be sent by an {nameof(Employer)} to a {nameof(Student)}.");
            if (dto.VacancyId == null)
                return (null, StatusCodes.Status400BadRequest, $"{nameof(Vacancy)}Id is required for {nameof(Invitation.Type.EmployerToStudent)} {nameof(Invitation)}.");
        }
        else if (dto.Type == nameof(Invitation.Type.StudentToEmployer))
        {
            if (sender is not Student || receiver is not Employer)
                return (null, StatusCodes.Status400BadRequest, $"{nameof(Invitation.Type.StudentToEmployer)} {nameof(Invitation)} must be sent by a {nameof(Student)} to an {nameof(Employer)}.");
            if (dto.ResumeId == null)
                return (null, StatusCodes.Status400BadRequest, $"{nameof(Resume)}Id is required for {nameof(Invitation.Type.StudentToEmployer)} {nameof(Invitation)}.");
        }

        if (await _context.Invitations.AnyAsync(i => i.SenderId == senderId && i.ReceiverId == dto.ReceiverId && (i.VacancyId == dto.VacancyId || i.ResumeId == dto.ResumeId)))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Invitation), $"senderId, receiverId, {nameof(Vacancy)}Id/{nameof(Resume)}Id"));

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            Type = Enum.Parse<Invitation.InvitationType>(dto.Type),
            Status = Invitation.InvitationStatus.Sent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var chat = await _context.Chats
        .FirstOrDefaultAsync(c => (c.User1Id == senderId && c.User2Id == dto.ReceiverId) || (c.User1Id == dto.ReceiverId && c.User2Id == senderId));

        if (chat == null)
        {
            chat = new Chat
            {
                Id = Guid.NewGuid(),
                User1Id = senderId,
                User2Id = dto.ReceiverId,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };
            _context.Chats.Add(chat);
        }
        else
        {
            chat.LastMessageAt = DateTime.UtcNow;
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chat.Id,
            SenderId = senderId,
            Content = $"{nameof(Invitation)}: {dto.Type} ({nameof(Vacancy)}Id: {dto.VacancyId}, {nameof(Resume)}Id: {dto.ResumeId})",
            InvitationId = invitation.Id,
            SentAt = DateTime.UtcNow
        };

        _context.Invitations.Add(invitation);
        _context.Messages.Add(message);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Invitation>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToInvitationDto(invitation), null, null);
    }

    /// <summary>
    /// Updates the status of an existing invitation.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="receiverId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateInvitationStatusAsync(Guid receiverId, Guid id, UpdateInvitationDto dto)
    {
        var invitation = await _context.Invitations.FindAsync(id);
        if (invitation == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Invitation)));
        if (invitation.ReceiverId != receiverId)
            return (false, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("update status", nameof(Invitation)));
        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return (false, StatusCodes.Status409Conflict, $"{nameof(Invitation)} status cannot be changed.");

        invitation.Status = Enum.Parse<Invitation.InvitationStatus>(dto.Status);
        invitation.UpdatedAt = DateTime.UtcNow;
        return await SaveChangesAsync<Invitation>();
    }
}
