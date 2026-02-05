using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseNotificationService(StudHunterDbContext context) : BaseService(context)
{
    protected IQueryable<Notification> GetBaseNotificationQuery(Guid userId) =>
        _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);
}
