using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseRegistrationManager(StudHunterDbContext context)
{
    protected readonly StudHunterDbContext _context = context;

    protected void RecalculateStudentStageInternal(Student student)
    {
        if (student.IsDeleted)
        {
            student.RegistrationStage = User.AccountStatus.Anonymous;
            return;
        }

        bool isBasicProfileFilled =
            !string.IsNullOrWhiteSpace(student.FirstName) && student.FirstName != UserDefaultNames.DefaultFirstName &&
            !string.IsNullOrWhiteSpace(student.LastName) && student.LastName != UserDefaultNames.DefaultLastName &&
            student.BirthDate.HasValue &&
            student.Gender.HasValue &&
            student.CityId.HasValue;

        bool isEducationFilled =
            student.StudyPlan != null &&
            !student.StudyPlan.IsDeleted &&
            student.StudyPlan.UniversityId.HasValue &&
            student.StudyPlan.FacultyId.HasValue &&
            student.StudyPlan.DepartmentId.HasValue &&
            student.StudyPlan.StudyDirectionId.HasValue &&
            student.StudyPlan.CourseNumber > 0;

        bool hasActiveResume = student.Resume != null && !student.Resume.IsDeleted;

        if (isBasicProfileFilled && isEducationFilled && hasActiveResume)
            student.RegistrationStage = User.AccountStatus.FullyActivated;
        else if (isBasicProfileFilled)
            student.RegistrationStage = User.AccountStatus.ProfileFilled;
        else
            student.RegistrationStage = User.AccountStatus.Anonymous;
    }

    public void RecalculateEmployerStageInternal(Employer employer)
    {
        if (employer.IsDeleted)
        {
            employer.RegistrationStage = User.AccountStatus.Anonymous;
            return;
        }

        bool isDataValid = !string.IsNullOrWhiteSpace(employer.Name) &&
                           employer.Name != UserDefaultNames.DefaultCompanyName &&
                           !string.IsNullOrWhiteSpace(employer.ContactEmail);

        if (!isDataValid)
        {
            employer.RegistrationStage = User.AccountStatus.Anonymous;
            return;
        }

        if (employer.RegistrationStage == User.AccountStatus.FullyActivated)
        {
            var entry = _context.Entry(employer);
            bool isRadicalChange = entry.State == EntityState.Modified && entry.Property(e => e.Name).IsModified;

            if (employer.OrganizationDetails != null)
            {
                var orgEntry = _context.Entry(employer.OrganizationDetails);
                if (orgEntry.State == EntityState.Modified || orgEntry.State == EntityState.Added)
                {
                    isRadicalChange |= orgEntry.Property(o => o.Inn).IsModified ||
                                       orgEntry.Property(o => o.Ogrn).IsModified;
                }
            }

            if (isRadicalChange)
            {
                employer.RegistrationStage = User.AccountStatus.ProfileFilled;
            }
            return;
        }

        if (employer.RegistrationStage == User.AccountStatus.Anonymous && isDataValid)
        {
            employer.RegistrationStage = User.AccountStatus.ProfileFilled;
        }
    }
}
