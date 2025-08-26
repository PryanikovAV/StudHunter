using StudHunter.API.ModelsDto.Invitation;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseInvitationService(StudHunterDbContext context) : BaseService(context)
{
    protected InvitationDto MapToInvitationDto(Invitation invitation)
    {
        return new InvitationDto
        {
            Id = invitation.Id,
            SenderId = invitation.SenderId,
            SenderEmail = invitation.Sender.IsDeleted ? "[Deleted Account]" : invitation.Sender.Email,
            ReceiverId = invitation.ReceiverId,
            ReceiverEmail = invitation.Receiver.IsDeleted ? "[Deleted Account]" : invitation.Receiver.Email,
            VacancyId = invitation.VacancyId,
            ResumeId = invitation.ResumeId,
            Type = invitation.Type.ToString(),
            Status = invitation.Status.ToString(),
            CreatedAt = invitation.CreatedAt,
            UpdatedAt = invitation.UpdatedAt,
            VacancyStatus = invitation.Vacancy != null ? (invitation.Vacancy.IsDeleted ? "Deleted" : "Active") : null,
            ResumeStatus = invitation.Resume != null ? (invitation.Resume.IsDeleted ? "Deleted" : "Active") : null
        };
    }
}
