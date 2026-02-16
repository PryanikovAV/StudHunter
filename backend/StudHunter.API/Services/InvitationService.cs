using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IInvitationService
{
    Task<Result<InvitationDto>> CreateInvitationAsync(Guid senderId, CreateInvitationDto dto, Invitation.InvitationType type);
    Task<Result<bool>> ChangeStatusAsync(Guid userId, Guid invitationId, Invitation.InvitationStatus newStatus);
    Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForStudentAsync(Guid studentId, InvitationSearchFilter filter);
    Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForEmployerAsync(Guid employerId, InvitationSearchFilter filter);
}

public class InvitationService(StudHunterDbContext context,
    INotificationService notificationService,
    IRegistrationManager registrationManager)
    : BaseInvitationService(context, notificationService, registrationManager), IInvitationService
{
    public async Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForStudentAsync(Guid studentId, InvitationSearchFilter filter) =>
        await GetInvitationsInternalAsync(studentId, filter);

    public async Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForEmployerAsync(Guid employerId, InvitationSearchFilter filter) =>
        await GetInvitationsInternalAsync(employerId, filter);

    public async Task<Result<InvitationDto>> CreateInvitationAsync(Guid senderId, CreateInvitationDto dto, Invitation.InvitationType type)
    {
        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null)
            return Result<InvitationDto>.Failure(ErrorMessages.EntityNotFound(nameof(User)), StatusCodes.Status404NotFound);

        var stageSenderCheck = EnsureCanPerform(sender, UserAction.SendInvitation);
        if (!stageSenderCheck.IsSuccess)
            return Result<InvitationDto>.Failure(stageSenderCheck.ErrorMessage!, stageSenderCheck.StatusCode);

        var blackListCheck = await EnsureCommunicationAllowedAsync(senderId, dto.ReceiverId);
        if (!blackListCheck.IsSuccess)
            return Result<InvitationDto>.Failure(blackListCheck.ErrorMessage!, blackListCheck.StatusCode);

        var existing = await _context.Invitations.AnyAsync(i =>
            i.SenderId == senderId &&
            i.ReceiverId == dto.ReceiverId &&
            i.VacancyId == dto.VacancyId &&
            i.ResumeId == dto.ResumeId &&
            i.Status == Invitation.InvitationStatus.Sent);

        if (existing)
            return Result<InvitationDto>.Failure("Вы уже отправили запрос, ожидайте ответа");

        string? vacancyTitle = null;
        if (dto.VacancyId.HasValue)
        {
            vacancyTitle = await _context.Vacancies
                .Where(v => v.Id == dto.VacancyId)
                .Select(v => v.Title)
                .FirstOrDefaultAsync();

            if (vacancyTitle == null)
                return Result<InvitationDto>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);
        }

        var invitation = new Invitation
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            Message = dto.Message?.Trim(),
            Type = type,
            SnapshotSenderName = UserDisplayHelper.GetUserDisplayName(sender),
            SnapshotVacancyTitle = vacancyTitle
        };

        await PopulateSnapshotsAsync(invitation);
        _context.Invitations.Add(invitation);

        var result = await SaveChangesAsync<Invitation>();

        if (result.IsSuccess)
        {
            string title = type == Invitation.InvitationType.Offer
                ? "Новое предложение о работе"
                : "Новый отклик на вакансию";

            await _notificationService.SendAsync(
                dto.ReceiverId,
                title,
                $"По вакансии: {vacancyTitle ?? "Без названия"}",
                Notification.NotificationType.InvitationIncome,
                invitation.Id,
                senderId: senderId
            );
        }

        return result.IsSuccess
            ? Result<InvitationDto>.Success(InvitationMapper.ToDto(invitation))
            : Result<InvitationDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> ChangeStatusAsync(Guid userId, Guid invitationId, Invitation.InvitationStatus newStatus)
    {
        var invitation = await _context.Invitations.FindAsync(invitationId);
        if (invitation == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return Result<bool>.Failure("Этот запрос уже обработан или просрочен");

        if (newStatus == Invitation.InvitationStatus.Cancelled)
        {
            if (invitation.SenderId != userId)
                return Result<bool>.Failure("Вы не можете отозвать чужое приглашение", StatusCodes.Status403Forbidden);
        }
        else
        {
            if (invitation.ReceiverId != userId)
                return Result<bool>.Failure("Вы не можете отвечать на это приглашение", StatusCodes.Status403Forbidden);
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(User)), StatusCodes.Status404NotFound);

        if (newStatus == Invitation.InvitationStatus.Accepted)
        {
            var accessCheck = EnsureCanPerform(user, UserAction.AcceptInvitation);
            if (!accessCheck.IsSuccess)
                return accessCheck;
        }

        invitation.Status = newStatus;
        invitation.UpdatedAt = DateTime.UtcNow;

        var result = await SaveChangesAsync<Invitation>();

        if (result.IsSuccess)
        {
            Guid receiverOfNotify = (userId == invitation.SenderId)
                ? invitation.ReceiverId
                : invitation.SenderId;

            string statusText = newStatus switch
            {
                Invitation.InvitationStatus.Accepted => "принято",
                Invitation.InvitationStatus.Rejected => "отклонено",
                Invitation.InvitationStatus.Cancelled => "отозвано",
                _ => "обновлено"
            };

            await _notificationService.SendAsync(
                receiverOfNotify,
                "Изменение статуса",
                $"Приглашение по вакансии \"{invitation.SnapshotVacancyTitle}\" было {statusText}.",
                Notification.NotificationType.InvitationStatus,
                invitationId,
                senderId: userId
            );
        }

        return result;
    }
}