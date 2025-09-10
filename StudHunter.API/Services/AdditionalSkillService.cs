using StudHunter.API.Services.BaseServices;
using StudHunter.DB.Postgres;

namespace StudHunter.API.Services;

public class AdditionalSkillService(StudHunterDbContext context) : BaseAdditionalSkillService(context)
{

}
