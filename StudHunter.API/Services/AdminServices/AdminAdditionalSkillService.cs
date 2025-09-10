using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services.AdminServices;

public class AdminAdditionalSkillService(StudHunterDbContext context) : BaseAdditionalSkillService(context)
{

}
