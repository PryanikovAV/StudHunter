using System.ComponentModel.DataAnnotations;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto;

public record CreateResponseDto(
    [Required] Guid VacancyId,
    [Required] Guid ResumeId,
    [StringLength(1000)] string? Message
);

public record CreateOfferDto(
    [Required] Guid StudentId,
    Guid? VacancyId,
    [StringLength(1000)] string? Message
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
    PaginationParams? Paging = null
)
{
    public PaginationParams SafePaging => Paging ?? new PaginationParams();
};

public record ApplyToVacancyRequest(Guid ResumeId, string? Message);

public static class InvitationMapper
{
    public static InvitationCardDto ToCardDto(Invitation invitation, Guid currentUserId)
    {
        string direction = "Unknown";
        if (invitation.StudentId == currentUserId)
        {
            direction = invitation.Type == Invitation.InvitationType.Response ? "Outgoing" : "Incoming";
        }
        else if (invitation.EmployerId == currentUserId)
        {
            direction = invitation.Type == Invitation.InvitationType.Offer ? "Outgoing" : "Incoming";
        }

        var candidateDto = new InvitationCandidateDto(
            StudentId: invitation.StudentId,
            FullName: invitation.Student != null
                ? UserDisplayHelper.GetUserDisplayName(invitation.Student)
                : invitation.SnapshotStudentName ?? "Неизвестный кандидат",
            AvatarUrl: invitation.Student?.AvatarUrl,
            Age: UserDisplayHelper.CalculateAge(invitation.Student?.BirthDate),
            CourseNumber: invitation.Student?.StudyPlan?.CourseNumber,
            UniversityAbbreviation: invitation.Student?.StudyPlan?.University?.Abbreviation,
            SpecialtyName: invitation.Student?.StudyPlan?.StudyDirection?.Name,
            ResumeId: invitation.ResumeId,
            ResumeTitle: invitation.Resume?.Title,
            Skills: invitation.Resume?.AdditionalSkills
                ?.Select(ras => ras.AdditionalSkill.Name)
                .ToList() ?? new List<string>()
        );

        var jobDto = new InvitationJobDto(
            EmployerId: invitation.EmployerId,
            CompanyName: invitation.Employer != null
                ? UserDisplayHelper.GetUserDisplayName(invitation.Employer)
                : invitation.SnapshotEmployerName ?? "Неизвестная компания",
            AvatarUrl: invitation.Employer?.AvatarUrl,
            VacancyId: invitation.VacancyId,
            VacancyTitle: invitation.Vacancy?.Title ?? invitation.SnapshotVacancyTitle,
            Salary: invitation.Vacancy?.Salary
        );

        return new InvitationCardDto(
            Id: invitation.Id,
            Status: invitation.Status.ToString(),
            Type: invitation.Type.ToString(),
            Direction: direction,
            SentAt: invitation.CreatedAt,
            Message: invitation.Message,
            Candidate: candidateDto,
            Job: jobDto
        );
    }
}