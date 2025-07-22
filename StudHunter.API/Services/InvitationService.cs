using Microsoft.EntityFrameworkCore;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class InvitationService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    public UserAchievementService _userAchievementService = userAchievementService;

    public async Task<IEnumerable<InvitationDto>> GetInvitationsByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Invitations.Where(i => i.SenderId == userId).Include(i => i.Vacancy).Include(r => r.Resume)
        : _context.Invitations.Where(i => i.ReceiverId == userId).Include(i => i.Vacancy).Include(r => r.Resume);

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
            UpdatedAt = i.UpdatedAt,
            VacancyStatus = i.Vacancy != null ? (i.Vacancy.IsDeleted ? "Deleted" : "Active") : null,
            ResumeStatus = i.Resume != null ? (i.Resume.IsDeleted ? "Deleted" : "Active") : null
        })
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync();
    }

    public async Task<(InvitationDto? Invitation, string? Error)> CreateInvitationAsync(Guid senderId, CreateInvitationDto dto)
    {
        if (senderId == dto.ReceiverId)
            return (null, "Sender and receiver cannot be the same");

        if (dto.VacancyId == null && dto.ResumeId == null)
            return (null, "Either VacancyId or ResumeId must be provided");

        if (dto.VacancyId != null && dto.ResumeId != null)
            return (null, "Only VacancyId or ResumeId can be provided");

        var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == senderId);
        var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.ReceiverId);

        if (sender == null)
            return (null, "Sender not found");

        if (receiver == null)
            return (null, "Receiver not found");

        if (dto.VacancyId != null && await _context.Vacancies.AnyAsync(v => v.Id == dto.VacancyId))
            return (null, "Vacancy not found");

        if (dto.ResumeId != null && await _context.Resumes.AnyAsync(r => r.Id == dto.ResumeId))
            return (null, "Resume not found");

        if (dto.Type == "EmployerToStudent")
        {
            if (sender is not Employer || receiver is not Student)
                return (null, "EmployerToStudent invitaion must be sent by Employer to Student");

            if (dto.VacancyId == null)
                return (null, "VacancyId is required for EmployerToStudent invitation");
        }
        else if (dto.Type == "StudentToEmployer")
        {
            if (sender is not Student || receiver is not Employer)
                return (null, "StudentToEmployer invitaion must be sent by Student to Employer");
            if (dto.ResumeId == null)
                return (null, "ResumeId is required for StudentToEmployer invitation");
        }

        if (await _context.Invitations.AnyAsync(i => i.SenderId == senderId && i.ReceiverId == dto.ReceiverId
        && (i.VacancyId == dto.VacancyId || i.ResumeId == dto.ResumeId)))
            return (null, "Invitation already exists");

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            VacancyId = dto.VacancyId,
            ResumeId = dto.ResumeId,
            Type = Enum.Parse<Invitation.InvitationType>(dto.Type),
            Message = dto.Message,
            Status = Invitation.InvitationStatus.Sent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Invitations.Add(invitation);

        var (success, error) = await SaveChangesAsync("create", "invitation");
        if (!success)
            return (null, error);

        // ===== Achievement =====
        await _userAchievementService.CheckAndGrantInvitationAchievementAsync(senderId);
        // ===== Achievement =====

        return (new InvitationDto
        {
            Id = invitation.Id,
            SenderId = invitation.SenderId,
            ReceiverId = invitation.ReceiverId,
            VacancyId = invitation.VacancyId,
            ResumeId = invitation.ResumeId,
            Type = invitation.Type.ToString(),
            Message = invitation.Message,
            Status = invitation.Status.ToString(),
            CreatedAt = invitation.CreatedAt,
            UpdatedAt = invitation.UpdatedAt,
            VacancyStatus = invitation.VacancyId.HasValue ?
                (await _context.Vacancies.AnyAsync(v => v.Id == invitation.VacancyId && !v.IsDeleted) ? "Active" : "Deleted") : null,
            ResumeStatus = invitation.ResumeId.HasValue ?
                (await _context.Resumes.AnyAsync(r => r.Id == invitation.ResumeId && !r.IsDeleted) ? "Active" : "Deleted") : null
        }, null);
    }

    public async Task<(bool Success, string? Error)> UpdateInvitationStatusAsync(Guid id, Guid receiverId, UpdateInvitationDto dto)
    {
        var invitation = await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id && i.ReceiverId == receiverId);

        if (invitation == null)
            return (false, "Invitation not found or user is not the receiver");

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return (false, "Invitation status cannot be changed");

        invitation.Status = Enum.Parse<Invitation.InvitationStatus>(dto.Status);
        invitation.UpdatedAt = DateTime.UtcNow;

        return await SaveChangesAsync("update", "invitation");
    }
}
