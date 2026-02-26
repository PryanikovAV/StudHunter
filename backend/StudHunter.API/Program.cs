using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudHunter.API.Extensions;
using StudHunter.API.Hubs;
using StudHunter.API.Infrastructure;
using StudHunter.API.Services;
using StudHunter.API.Services.AdminServices;
using StudHunter.API.Services.AuthService;
using StudHunter.API.Services.Background;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudHunterDbContext>(options =>
    options.UseNpgsql(connectionString));

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
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddHostedService<InvitationCleanupService>();

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

builder.Services.AddScoped<DictionarySeederService>();  // <-- Заполнение словарей

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())   // <-- Заполнение словарей
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StudHunterDbContext>();
        context.Database.Migrate();

        var seeder = services.GetRequiredService<DictionarySeederService>();
        await seeder.SeedAllAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации базы данных");
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StudHunterDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

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
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();