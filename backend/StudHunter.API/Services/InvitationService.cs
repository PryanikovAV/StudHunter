using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IInvitationService
{
    Task<Result<InvitationCardDto>> CreateResponseAsync(Guid studentId, CreateResponseDto dto);
    Task<Result<InvitationCardDto>> CreateOfferAsync(Guid employerId, CreateOfferDto dto);

    Task<Result<bool>> ChangeStatusAsync(Guid userId, Guid invitationId, Invitation.InvitationStatus newStatus);
    Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForStudentAsync(Guid studentId, InvitationSearchFilter filter);
    Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForEmployerAsync(Guid employerId, InvitationSearchFilter filter);
}

public class InvitationService(StudHunterDbContext context,
    INotificationService notificationService,
    IRegistrationManager registrationManager)
    : BaseInvitationService(context, notificationService, registrationManager), IInvitationService
{
    public async Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForStudentAsync(Guid studentId, InvitationSearchFilter filter)
    {
        var query = GetFullInvitationQuery().Where(i => i.StudentId == studentId);

        if (filter.Incoming)
            query = query.Where(i => i.Type == Invitation.InvitationType.Offer);
        else
            query = query.Where(i => i.Type == Invitation.InvitationType.Response);

        return await ExecutePagedQueryAsync(query, filter, studentId);
    }

    public async Task<Result<PagedResult<InvitationCardDto>>> GetInvitationsForEmployerAsync(Guid employerId, InvitationSearchFilter filter)
    {
        var query = GetFullInvitationQuery().Where(i => i.EmployerId == employerId);

        if (filter.Incoming)
            query = query.Where(i => i.Type == Invitation.InvitationType.Response);
        else
            query = query.Where(i => i.Type == Invitation.InvitationType.Offer);

        return await ExecutePagedQueryAsync(query, filter, employerId);
    }

    public async Task<Result<InvitationCardDto>> CreateResponseAsync(Guid studentId, CreateResponseDto dto)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == dto.VacancyId);

        if (vacancy == null)
            return Result<InvitationCardDto>.Failure(ErrorMessages.EntityNotFound(nameof(Vacancy)), StatusCodes.Status404NotFound);

        var existing = await _context.Invitations.AnyAsync(i =>
            i.StudentId == studentId && i.VacancyId == dto.VacancyId && i.Status == Invitation.InvitationStatus.Sent);

        if (existing) return Result<InvitationCardDto>.Failure("Вы уже откликнулись на эту вакансию", StatusCodes.Status400BadRequest);

        var invitation = new Invitation
        {
            StudentId = studentId,
            EmployerId = vacancy.EmployerId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            Message = dto.Message,
            Type = Invitation.InvitationType.Response
        };

        return await SaveAndNotifyAsync(invitation, vacancy.EmployerId, "Новый отклик на вакансию", studentId);
    }

    public async Task<Result<InvitationCardDto>> CreateOfferAsync(Guid employerId, CreateOfferDto dto)
    {
        var existing = await _context.Invitations.AnyAsync(i =>
            i.EmployerId == employerId && i.StudentId == dto.StudentId &&
            i.VacancyId == dto.VacancyId && i.Status == Invitation.InvitationStatus.Sent);

        if (existing) return Result<InvitationCardDto>.Failure("Вы уже отправили приглашение этому студенту", StatusCodes.Status400BadRequest);

        var invitation = new Invitation
        {
            EmployerId = employerId,
            StudentId = dto.StudentId,
            VacancyId = dto.VacancyId,
            Message = dto.Message,
            Type = Invitation.InvitationType.Offer
        };

        return await SaveAndNotifyAsync(invitation, dto.StudentId, "Новое приглашение", employerId);
    }

    public async Task<Result<bool>> ChangeStatusAsync(Guid userId, Guid invitationId, Invitation.InvitationStatus newStatus)
    {
        var invitation = await _context.Invitations.FindAsync(invitationId);
        if (invitation == null) return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(Invitation)), StatusCodes.Status404NotFound);

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return Result<bool>.Failure("Этот запрос уже обработан или просрочен");

        bool isStudent = invitation.StudentId == userId;
        bool isEmployer = invitation.EmployerId == userId;
        bool isInitiator = (invitation.Type == Invitation.InvitationType.Response && isStudent) ||
                           (invitation.Type == Invitation.InvitationType.Offer && isEmployer);

        if (newStatus == Invitation.InvitationStatus.Cancelled && !isInitiator)
            return Result<bool>.Failure("Вы не можете отозвать чужое приглашение", StatusCodes.Status403Forbidden);

        if ((newStatus == Invitation.InvitationStatus.Accepted || newStatus == Invitation.InvitationStatus.Rejected) && isInitiator)
            return Result<bool>.Failure("Вы не можете отвечать на собственное приглашение", StatusCodes.Status403Forbidden);

        invitation.Status = newStatus;
        invitation.UpdatedAt = DateTime.UtcNow;

        var result = await SaveChangesAsync<Invitation>();

        if (result.IsSuccess)
        {
            Guid receiverOfNotify = isStudent ? invitation.EmployerId : invitation.StudentId;
            await _notificationService.SendAsync(receiverOfNotify, "Изменение статуса", $"Статус приглашения изменен на {newStatus}",
                Notification.NotificationType.InvitationStatus, invitationId, senderId: userId);
        }

        return result;
    }

    private async Task<Result<PagedResult<InvitationCardDto>>> ExecutePagedQueryAsync(IQueryable<Invitation> query, InvitationSearchFilter filter, Guid currentUserId)
    {
        if (filter.Status.HasValue)
            query = query.Where(i => i.Status == filter.Status);

        query = query.OrderByDescending(i => i.CreatedAt);

        var paged = await query.ToPagedResultAsync(filter.SafePaging);

        var dtos = paged.Items.Select(i => InvitationMapper.ToCardDto(i, currentUserId)).ToList();

        return Result<PagedResult<InvitationCardDto>>.Success(new PagedResult<InvitationCardDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }

    private async Task<Result<InvitationCardDto>> SaveAndNotifyAsync(Invitation invitation, Guid receiverId, string title, Guid currentUserId)
    {
        await PopulateSnapshotsAsync(invitation);
        _context.Invitations.Add(invitation);
        var result = await SaveChangesAsync<Invitation>();

        if (result.IsSuccess)
        {
            await _notificationService.SendAsync(receiverId, title,
                $"По вакансии: {invitation.SnapshotVacancyTitle ?? "Без названия"}", Notification.NotificationType.InvitationIncome, invitation.Id, senderId: currentUserId);

            var createdEntity = await GetFullInvitationQuery().FirstAsync(i => i.Id == invitation.Id);

            return Result<InvitationCardDto>.Success(InvitationMapper.ToCardDto(createdEntity, currentUserId));
        }

        return Result<InvitationCardDto>.Failure(result.ErrorMessage!);
    }
}