using Microsoft.EntityFrameworkCore;
using StudHunter.API.Common;
using StudHunter.API.ModelsDto.Invitation;
using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public class InvitationService(StudHunterDbContext context, UserAchievementService userAchievementService) : BaseService(context)
{
    public UserAchievementService _userAchievementService = userAchievementService;

    public async Task<(List<InvitationDto>? Entities, int? StatusCode, string? ErrorMessage)> GetInvitationsByUserAsync(Guid userId, bool sent = false)
    {
        var query = sent
        ? _context.Invitations.Where(i => i.SenderId == userId).Include(i => i.Vacancy).Include(r => r.Resume)
        : _context.Invitations.Where(i => i.ReceiverId == userId).Include(i => i.Vacancy).Include(r => r.Resume);

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
            UpdatedAt = i.UpdatedAt,
            VacancyStatus = i.Vacancy != null ? (i.Vacancy.IsDeleted ? "Deleted" : "Active") : null,
            ResumeStatus = i.Resume != null ? (i.Resume.IsDeleted ? "Deleted" : "Active") : null
        })
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync();

        return (invitations, null, null);
    }

    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> GetInvitationAsync(Guid id)
    {
        var invitation = await _context.Invitations
        .Where(i => i.Id == id)
        .Select(i => new InvitationDto
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
        }).FirstOrDefaultAsync();

        #region Serializers
        if (invitation == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Invitation"));
        #endregion

        return (invitation, null, null);
    }

    public async Task<(InvitationDto? Entity, int? StatusCode, string? ErrorMessage)> CreateInvitationAsync(Guid senderId, CreateInvitationDto dto)
    {
        #region Serializers
        if (senderId == dto.ReceiverId)
            return (null, StatusCodes.Status400BadRequest, "Sender and receiver cannot be the same.");

        if (dto.VacancyId == null && dto.ResumeId == null)
            return (null, StatusCodes.Status400BadRequest, "Either VacancyId or ResumeId must be provided.");

        if (dto.VacancyId != null && dto.ResumeId != null)
            return (null, StatusCodes.Status400BadRequest, "Only one of VacancyId or ResumeId can be provided.");

        var senderExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == senderId);
        if (senderExists == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Sender"));

        var receiverExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.ReceiverId);
        if (receiverExists == null)
            return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Receiver"));

        if (dto.VacancyId != null)
        {
            var vacancyExists = await _context.Vacancies.AnyAsync(v => v.Id == dto.VacancyId);
            if (vacancyExists == false)
                return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Vacancy"));
        }

        if (dto.ResumeId != null)
        {
            var resumeExists = await _context.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (resumeExists == false)
                return (null, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Resume"));
        }

        if (dto.Type == "EmployerToStudent")
        {
            if (senderExists is not Employer || receiverExists is not Student)
                return (null, StatusCodes.Status400BadRequest, "EmployerToStudent invitation must be sent by Employer to Student");
            if (dto.VacancyId == null)
                return (null, StatusCodes.Status400BadRequest, "VacancyId is required for EmployerToStudent invitation");
        }
        else if (dto.Type == "StudentToEmployer")
        {
            if (senderExists is not Student || receiverExists is not Employer)
                return (null, StatusCodes.Status400BadRequest, "StudentToEmployer invitation must be sent by Student to Employer");
            if (dto.ResumeId == null)
                return (null, StatusCodes.Status400BadRequest, "ResumeId is required for StudentToEmployer invitation");
        }

        if (await _context.Invitations.AnyAsync(i => i.SenderId == senderId && i.ReceiverId == dto.ReceiverId && (i.VacancyId == dto.VacancyId || i.ResumeId == dto.ResumeId)))
            return (null, StatusCodes.Status409Conflict, ErrorMessages.AlreadyExists("Invitation", "SenderId, ReceiverId, VacancyId/ResumeId"));
        #endregion

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

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Invitation");

        if (!success)
            return (null, statusCode, errorMessage);

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
        }, null, null);
    }

    public async Task<(bool Success, int? StatusCode, string? ErrorMessage)> UpdateInvitationStatusAsync(Guid id, Guid receiverId, UpdateInvitationDto dto)
    {
        var invitation = await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id && i.ReceiverId == receiverId);

        #region Serializers
        if (invitation == null)
            return (false, StatusCodes.Status404NotFound, ErrorMessages.NotFound("Invitation"));

        if (invitation.Status != Invitation.InvitationStatus.Sent)
            return (false, StatusCodes.Status409Conflict, "Invitation status cannot be changed");
        #endregion

        invitation.Status = Enum.Parse<Invitation.InvitationStatus>(dto.Status);
        invitation.UpdatedAt = DateTime.UtcNow;

        var (success, statusCode, errorMessage) = await SaveChangesAsync("Invitation");

        return (success, statusCode, errorMessage);
    }
}
