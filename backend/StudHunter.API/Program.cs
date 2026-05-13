using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudHunter.API.Extensions;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.API.Services;
using StudHunter.API.Services.AdminServices;
using StudHunter.API.Services.AuthService;
using StudHunter.API.Services.Background;
using StudHunter.API.Services.Files;
using StudHunter.API.Services.Pdf;
using StudHunter.API.Services.Search;
using StudHunter.API.Services.StatisticsService;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<StudHunterDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("StudHunter.DB.Postgres");

        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    }));

builder.Services.AddAuth(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["*"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

/* Обработчик ошибок, регистрация */
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

/* Сервисы авторизации и фоновые задачи */
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<InvitationCleanupService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

/* Сервис для работы с файлами */
builder.Services.AddScoped<IFileService, LocalFileService>();
builder.Services.AddScoped<IPdfService, QuestPdfService>();

/* Пользовательские сервисы */
builder.Services.AddScoped<IBlackListService, BlackListService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IDictionariesService, DictionariesService>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRegistrationManager, RegistrationManager>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();

/* Административные сервисы */
builder.Services.AddScoped<IAdminBlackListService, AdminBlackListService>();
builder.Services.AddScoped<IAdminChatService, AdminChatService>();
builder.Services.AddScoped<IAdminDictionariesService, AdminDictionariesService>();
builder.Services.AddScoped<IAdminEmployerService, AdminEmployerService>();
builder.Services.AddScoped<IAdminFavoriteService, AdminFavoriteService>();
builder.Services.AddScoped<IAdminInvitationService, AdminInvitationService>();
builder.Services.AddScoped<IAdminResumeService, AdminResumeService>();
builder.Services.AddScoped<IAdminStudentService, AdminStudentService>();
builder.Services.AddScoped<IAdminVacancyService, AdminVacancyService>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<StudHunterDbContext>();

        // --- ОЖИДАНИЕ DB> ---
        int retries = 5;
        while (retries > 0)
        {
            try
            {
                logger.LogInformation("применение миграций");
                await context.Database.MigrateAsync();
                logger.LogInformation("миграции применены");
                break;
            }
            catch (Exception)
            {
                retries--;
                if (retries == 0) throw;
                logger.LogWarning($"DB ещё не готова, ожидание 3 сек. Осталось попыток: {retries}");
                await Task.Delay(3000);
            }
        }
        // --- КОНЕЦ ЦИКЛА ---

        var hasher = services.GetRequiredService<IPasswordHasher>();

        var seeder = new StudHunter.DB.Postgres.Seeding.DbSeeder(
            context,
            password => hasher.HashPassword(password));
        
        await seeder.SeedAsync();

        if (!context.Administrators.Any())
        {
            var admin = new Administrator
            {
                Email = "admin@example.com",
                PasswordHash = hasher.HashPassword("password123"),
                FirstName = "Главный",
                LastName = "Администратор",
                RegistrationStage = User.AccountStatus.FullyActivated
            };
            context.Administrators.Add(admin);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "CRITICAL ERROR: Failed to initialize the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Шрифты для QuestPDF
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
QuestPDF.Drawing.FontManager.RegisterFont(File.OpenRead("Fonts/Roboto-Regular.ttf"));
QuestPDF.Drawing.FontManager.RegisterFont(File.OpenRead("Fonts/Roboto-Medium.ttf"));

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseStaticFiles();  // <- Для доступа к загруженным файлам

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();