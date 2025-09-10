using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.InvitationDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

/// <summary>
/// Service for managing invitations.
/// </summary>
public class InvitationService(StudHunterDbContext context, ChatService chatService, MessageService messageService) : BaseInvitationService(context)
{
    private readonly ChatService _chatService = chatService;
    private readonly MessageService _messageService = messageService;

    /// <summary>
    /// Retrieves invitations for a specific user, either sent or received.
    /// </summary>
    /// <param name="authUserId">The unique identifier (GUID) of the user.</param>
    /// <param name="sent">A boolean indicating whether to retrieve sent (true) or received (false) invitations.</param>
    /// <returns>A tuple containing a list of invitations, an optional status code, and an optional error message.</returns>
    public async Task<(List<InvitationDto>? Entities, int? StatusCode, string? ErrorMessage)> GetInvitationsByUserAsync(Guid authUserId, bool sent = false)
    {
        var user = await _context.Users.FindAsync(authUserId);
        if (user == null)
            return (null, StatusCodes.Status401Unauthorized, ErrorMessages.InvalidTokenUserId());

        var query = sent
            ? _context.Invitations.Where(i => i.SenderId == authUserId)
            : _context.Invitations.Where(i => i.ReceiverId == authUserId);

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
    /// <param name="invitationId">The unique identifier (GUID) of the invitation.</param>
    /// <param name="authuserId">The unique identifier (GUID) of the user (sender).</param>
    /// <returns>A tuple containing the invitation, an optional status code, and an optional error message.</returns>
    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> GetInvitationAsync(Guid authuserId, Guid invitationId)
    {
        var invitation = await _context.Invitations
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Vacancy)
            .Include(i => i.Resume)
            .FirstOrDefaultAsync(i => i.Id == invitationId && (i.SenderId == authuserId || i.ReceiverId == authuserId));

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
            if (vacancy.IsDeleted)
                return (null, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Vacancy), nameof(UpdateInvitationStatusAsync)));
        }

        if (dto.ResumeId != null)
        {
            var resume = await _context.Resumes.FindAsync(dto.ResumeId);
            if (resume == null)
                return (null, StatusCodes.Status404NotFound, ErrorMessages.EntityNotFound(nameof(Resume)));
            if (resume.StudentId != senderId)
                return (null, StatusCodes.Status403Forbidden, ErrorMessages.RestrictOwnProfileAction("respond using", nameof(Resume)));
            if (resume.IsDeleted)
                return (null, StatusCodes.Status410Gone, ErrorMessages.EntityAlreadyDeleted(nameof(Resume), nameof(UpdateInvitationStatusAsync)));
        }

        if (sender is Student && dto.VacancyId == null)
            return (null, StatusCodes.Status400BadRequest, $"{nameof(Student)} can only respond to {nameof(Vacancy)}.");
        if (sender is Employer && dto.ResumeId == null)
            return (null, StatusCodes.Status400BadRequest, $"{nameof(Employer)} can only invite to {nameof(Resume)}.");

        if (await _context.Invitations.AnyAsync(i => i.SenderId == senderId && i.ReceiverId == dto.ReceiverId && (i.VacancyId == dto.VacancyId || i.ResumeId == dto.ResumeId)))
            return (null, StatusCodes.Status409Conflict, "Invitation already exists for this sender, receiver, and entity.");

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            Status = Invitation.InvitationStatus.Sent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var (chat, chatStatusCode, chatErrorMessage) = await _chatService.CreateChatAsync(senderId, dto.ReceiverId);
        if (chat == null)
            return (null, chatStatusCode, chatErrorMessage);

        var (message, messageStatusCode, messageErrorMessage) = await _messageService.CreateInvitationMessageAsync(senderId, chat.Id, invitation.Id, dto.VacancyId, dto.ResumeId);
        if (message == null)
            return (null, messageStatusCode, messageErrorMessage);

        _context.Invitations.Add(invitation);

        var (success, statusCode, errorMessage) = await SaveChangesAsync<Invitation>();

        if (!success)
            return (null, statusCode, errorMessage);

        return (MapToInvitationDto(invitation), null, null);
    }

    /// <summary>
    /// Updates the status of an existing invitation.
    /// </summary>
    /// <param name="invitationId"></param>
    /// <param name="receiverId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateInvitationStatusAsync(Guid receiverId, Guid invitationId, UpdateInvitationDto dto)
    {
        var invitation = await _context.Invitations.FindAsync(invitationId);
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
