using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

public class AdminInvitationService(StudHunterDbContext context, UserAchievementService userAchievementService)
: InvitationService(context, userAchievementService)
{
    public async Task<(List<InvitationDto>? Entities, int? StatusCode, string? ErrorMessage)> GetAllInvitationsAsync()
    {
        var invitations = await _context.Invitations.Select(i => new InvitationDto
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
            UpdatedAt = i.UpdatedAt
        })
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync();

        return (invitations, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> DeleteInvitationAsync(Guid id)
    {
        return await DeleteEntityAsync<Invitation>(id, hardDelete: true);
    }
}
