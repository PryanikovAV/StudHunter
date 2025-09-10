using StudHunter.API.ModelsDto.InvitationDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseInvitationService(StudHunterDbContext context) : BaseService(context)
{
    protected InvitationDto MapToInvitationDto(Invitation invitation)
    {
        var entitySummary = invitation.VacancyId != null ? new EntitySummaryDto
        {
            Id = invitation.VacancyId,
            Type = nameof(Vacancy),
            Title = invitation.Vacancy?.Title,
            Status = invitation.Vacancy?.IsDeleted == true ? "Deleted" : "Active"
        } : invitation.ResumeId != null ? new EntitySummaryDto
        {
            Id = invitation.ResumeId,
            Type = nameof(Resume),
            Title = invitation.Resume?.Title,
            Status = invitation.Resume?.IsDeleted == true ? "Deleted" : "Active"
        } : null;

        return new InvitationDto
        {
            Id = invitation.Id,
            SenderId = invitation.SenderId,
            SenderEmail = invitation.Sender.IsDeleted ? "[Deleted Account]" : invitation.Sender.Email,
            ReceiverId = invitation.ReceiverId,
            ReceiverEmail = invitation.Receiver.IsDeleted ? "[Deleted Account]" : invitation.Receiver.Email,
            Entity = entitySummary,
            Status = invitation.Status.ToString(),
            CreatedAt = invitation.CreatedAt,
            UpdatedAt = invitation.UpdatedAt,
        };
    }
}
