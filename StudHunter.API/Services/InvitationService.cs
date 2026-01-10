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
    Task<Result<PagedResult<InvitationDto>>> GetInvitationsAsync(Guid userId, InvitationSearchFilter filter);
}

public class InvitationService(StudHunterDbContext context, INotificationService notificationService)
    : BaseInvitationService(context, notificationService), IInvitationService
{
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

        var invitation = new Invitation
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            Message = dto.Message?.Trim(),
            Type = type
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
                $"Вы получили новое сообщение: {invitation.SnapshotVacancyTitle ?? "Просмотрите детали в личном кабинете"}",
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
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(User)), StatusCodes.Status404NotFound);

        var invitation = await _context.Invitations.FindAsync(invitationId);
        if (invitation == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return Result<bool>.Failure("Этот запрос уже обработан или просрочен");

        if (newStatus == Invitation.InvitationStatus.Accepted)
        {
            var accessCheck = EnsureCanPerform(user, UserAction.AcceptInvitation);
            if (!accessCheck.IsSuccess)
                return accessCheck;
        }

        if ((newStatus == Invitation.InvitationStatus.Accepted || newStatus == Invitation.InvitationStatus.Rejected)
            && invitation.ReceiverId != userId)
            return Result<bool>.Failure("Вы не можете отвечать на это приглашение", StatusCodes.Status403Forbidden);

        if (newStatus == Invitation.InvitationStatus.Cancelled && invitation.SenderId != userId)
            return Result<bool>.Failure("Вы не можете отозвать чужое приглашение", StatusCodes.Status403Forbidden);

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
                Invitation.InvitationStatus.Cancelled => "отозвано отправителем",
                _ => "обновлено"
            };

            await _notificationService.SendAsync(
                receiverOfNotify,
                "Обновление статуса взаимодействия",
                $"Ваш запрос по вакансии \"{invitation.SnapshotVacancyTitle}\" был: {statusText}.",
                Notification.NotificationType.InvitationStatus,
                invitationId,
                senderId: userId
            );
        }

        return result;
    }

    public async Task<Result<PagedResult<InvitationDto>>> GetInvitationsAsync(Guid userId, InvitationSearchFilter filter)
    {
        var query = GetFullInvitationQuery();
        var blockedIds = await GetBlockedUserIdsAsync(userId);

        query = filter.Incoming
            ? query.Where(i => i.ReceiverId == userId && !blockedIds.Contains(i.SenderId))
            : query.Where(i => i.SenderId == userId && !blockedIds.Contains(i.SenderId));

        if (filter.Status.HasValue)
            query = query.Where(i => i.Status == filter.Status.Value);

        if (filter.Type.HasValue)
            query = query.Where(i => i.Type == filter.Type.Value);

        var pagedInvitations = await query
            .OrderByDescending(i => i.CreatedAt)
            .ToPagedResultAsync(filter.Paging);

        var dtos = pagedInvitations.Items.Select(i => InvitationMapper.ToDto(i)).ToList();

        var pagedResult = new PagedResult<InvitationDto>(
            Items: dtos,
            TotalCount: pagedInvitations.TotalCount,
            PageNumber: pagedInvitations.PageNumber,
            PageSize: pagedInvitations.PageSize);

        return Result<PagedResult<InvitationDto>>.Success(pagedResult);
    }
}