using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto;

public record CreateInvitationDto(
    Guid ReceiverId,
    Guid? VacancyId,
    Guid? ResumeId,
    string? Message
);

public record InvitationDto(
    Guid Id,
    Guid SenderId,
    string SenderName,
    Guid ReceiverId,
    string ReceiverName,
    Guid? VacancyId,
    string? VacancyTitle,
    Guid? ResumeId,
    string Status,
    string Type,
    string? Message,
    DateTime CreatedAt,
    DateTime ExpiredAt,
    bool IsExpired
);

public record InvitationSearchFilter(
    Invitation.InvitationStatus? Status = null,
    Invitation.InvitationType? Type = null,
    bool Incoming = true,
    PaginationParams Paging = null!
);

public static class InvitationMapper
{
    public static InvitationDto ToDto(Invitation invitation)
    {
        bool isExpired = invitation.Status == Invitation.InvitationStatus.Sent
                         && DateTime.UtcNow > invitation.ExpiredAt;

        return new InvitationDto(
            invitation.Id,
            invitation.SenderId,
            invitation.SnapshotSenderName ?? "Удаленный пользователь",
            invitation.ReceiverId,
            invitation.SnapshotReceiverName ?? "Удаленный пользователь",

            invitation.VacancyId,
            invitation.Vacancy?.Title ?? invitation.SnapshotVacancyTitle ?? "Вакансия удалена",

            invitation.ResumeId,

            invitation.Status.ToString(),
            invitation.Type.ToString(),
            invitation.Message,
            invitation.CreatedAt,
            invitation.ExpiredAt,
            isExpired
        );
    }
}