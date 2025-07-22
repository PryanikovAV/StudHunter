using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

public class AdminInvitationService(StudHunterDbContext context) : BaseService(context)
{
    public async Task<IEnumerable<InvitationDto>> GetAllInvitationsAsync()
    {
        return await _context.Invitations.Select(i => new InvitationDto
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
    }

    public async Task<IEnumerable<InvitationDto>> GetInvitationsByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Invitations.Where(i => i.SenderId == userId)
        : _context.Invitations.Where(i => i.ReceiverId == userId);

        return await _context.Invitations.Select(i => new InvitationDto
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
    }

    public async Task<(bool Success, string? Error)> UpdateInvitationStatusAsync(Guid id, Guid receiverId, UpdateInvitationDto dto)
    {
        var invitation = await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id && i.ReceiverId == receiverId);

        if (invitation == null)
            return (false, "Invotation not found or user is not the receiver");

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return (false, "Invitation status cannot be changed");

        invitation.Status = Enum.Parse<Invitation.InvitationStatus>(dto.Status);
        invitation.UpdatedAt = DateTime.UtcNow;

        return await SaveChangesAsync("update", "Invitation");
    }

    public async Task<(bool Success, string? Error)> DeleteInvitationAsync(Guid id)
    {
        return await HardDeleteEntityAsync<Invitation>(id);
    }
}
