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
            return (null, StatusCodes.Status400BadRequest, "Either vacancyId or resumeId must be provided.");

        if (dto.VacancyId != null && dto.ResumeId != null)
            return (null, StatusCodes.Status400BadRequest, "Only one of vacancyId or resumeId can be provided.");

        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("sender"));

        var receiver = await _context.Users.FindAsync(dto.ReceiverId);
        if (receiver == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound("receiver"));

        if (dto.VacancyId != null)
        {
            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == dto.VacancyId);
            if (vacancy == null)
                return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Vacancy)));
            if (vacancy.EmployerId != senderId)
                return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("invite using", nameof(Vacancy)));
        }

        if (dto.ResumeId != null)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == dto.ResumeId);
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
            return (null, StatusCodes.Status409Conflict, ErrorMessages.EntityAlreadyExists(nameof(Invitation), "senderId, receiverId, vacancyId/resumeId"));

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
            Content = $"Invitation: {dto.Type} (VacancyId: {dto.VacancyId}, ResumeId: {dto.ResumeId})",
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
    /// <param name="id">The unique identifier (GUID) of the invitation.</param>
    /// <param name="receiverId">The unique identifier (GUID) of the receiver.</param>
    /// <param name="dto">The data transfer object containing the updated invitation status.</param>
    /// <returns>A tuple indicating whether the update was successful, an optional status code, and an optional error message.</returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateInvitationStatusAsync(Guid id, Guid receiverId, UpdateInvitationDto dto)
    {
        var invitation = await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id && i.ReceiverId == receiverId);

        if (invitation == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Invitation)));

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return (false, StatusCodes.Status409Conflict, $"{nameof(Invitation)} status cannot be changed.");

        invitation.Status = Enum.Parse<Invitation.InvitationStatus>(dto.Status);
        invitation.UpdatedAt = DateTime.UtcNow;

        return await SaveChangesAsync<Invitation>();
    }
}
