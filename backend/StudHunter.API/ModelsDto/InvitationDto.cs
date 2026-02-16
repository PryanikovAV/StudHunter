using StudHunter.API.Infrastructure;
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

public record InvitationCardDto(
    Guid Id,
    string Status,
    string Type,
    string Direction,
    DateTime SentAt,
    string? Message,

    InvitationCandidateDto Candidate,
    InvitationJobDto Job
);

public record InvitationCandidateDto(
    Guid StudentId,
    string FullName,
    string? AvatarUrl,
    int? Age,
    int? CourseNumber,
    string? UniversityAbbreviation,
    string? SpecialtyName,

    Guid? ResumeId,
    string? ResumeTitle,
    List<string> Skills
);

public record InvitationJobDto(
    Guid EmployerId,
    string CompanyName,
    string? AvatarUrl,
    Guid? VacancyId,
    string? VacancyTitle,
    decimal? Salary
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
    public static InvitationCardDto ToCardDto(Invitation invitation, Guid currentUserId)
    {
        var direction = invitation.SenderId == currentUserId ? "Outgoing" : "Incoming";

        Student? student = invitation.Resume?.Student
                           ?? invitation.Sender as Student
                           ?? invitation.Receiver as Student;

        Employer? employer = invitation.Vacancy?.Employer
                             ?? invitation.Sender as Employer
                             ?? invitation.Receiver as Employer;

        var candidateDto = new InvitationCandidateDto(
            StudentId: student?.Id ?? Guid.Empty,
            FullName: UserDisplayHelper.GetUserDisplayName(student!),
            AvatarUrl: student?.AvatarUrl,
            Age: UserDisplayHelper.CalculateAge(student?.BirthDate),
            CourseNumber: student?.StudyPlan?.CourseNumber,
            UniversityAbbreviation: student?.StudyPlan?.University?.Abbreviation,
            SpecialtyName: student?.StudyPlan?.StudyDirection?.Name,
            ResumeId: invitation.ResumeId,
            ResumeTitle: invitation.Resume?.Title,
            Skills: invitation.Resume?.AdditionalSkills
                .Select(ras => ras.AdditionalSkill.Name)
                .ToList() ?? new List<string>()
        );

        var jobDto = new InvitationJobDto(
            EmployerId: employer?.Id ?? Guid.Empty,
            CompanyName: UserDisplayHelper.GetUserDisplayName(employer!),
            AvatarUrl: employer?.AvatarUrl,
            VacancyId: invitation.VacancyId,
            VacancyTitle: invitation.Vacancy?.Title ?? invitation.SnapshotVacancyTitle,
            Salary: invitation.Vacancy?.Salary
        );

        return new InvitationCardDto(
            invitation.Id,
            invitation.Status.ToString(),
            invitation.Type.ToString(),
            direction,
            invitation.CreatedAt,
            invitation.Message,
            candidateDto,
            jobDto
        );
    }
}