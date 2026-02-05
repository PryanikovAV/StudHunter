using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.Background;

public class InvitationCleanupService(IServiceScopeFactory scopeFactory, ILogger<InvitationCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1);
            var delay = nextRun - now;

            logger.LogInformation("Проверка истекших приглашений запланирована через {Delay}", delay);
            await Task.Delay(delay, stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<StudHunterDbContext>();

                var expiredInvitations = await context.Invitations
                    .Where(i => i.Status == Invitation.InvitationStatus.Sent && i.ExpiredAt < DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                if (expiredInvitations.Any())
                {
                    foreach (var inv in expiredInvitations)
                    {
                        inv.Status = Invitation.InvitationStatus.Expired;
                        inv.UpdatedAt = DateTime.UtcNow;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                    logger.LogInformation("Обновлено статусов 'Просрочено': {Count}", expiredInvitations.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при автоматическом обновлении статусов приглашений");
            }
        }
    }
}