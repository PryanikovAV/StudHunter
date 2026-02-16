using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services;

public interface IRegistrationManager
{
    void RecalculateRegistrationStage(User user);
}

public class RegistrationManager(StudHunterDbContext context) : BaseRegistrationManager(context), IRegistrationManager
{
    public void RecalculateRegistrationStage(User user)
    {
        switch (user)
        {
            case Student student:
                RecalculateStudentStageInternal(student);
                break;

            case Employer employer:
                RecalculateEmployerStageInternal(employer);
                break;
            default:
                break;
        }
    }
}
