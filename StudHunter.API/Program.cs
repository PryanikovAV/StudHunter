using Microsoft.EntityFrameworkCore;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;
using StudHunter.API.Services.CommonService;
using StudHunter.DB.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudHunterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<AdministratorAchievementTemplateService>();
builder.Services.AddScoped<AdministratorCourseService>();
builder.Services.AddScoped<AdministratorEmployerService>();
builder.Services.AddScoped<AdministratorFacultyService>();
builder.Services.AddScoped<AdministratorFavoriteService>();
builder.Services.AddScoped<AdministratorInvitationService>();
builder.Services.AddScoped<AdministratorMessagesService>();
builder.Services.AddScoped<AdministratorResumeService>();
builder.Services.AddScoped<AdministratorService>();
builder.Services.AddScoped<AdministratorSpecialityService>();
builder.Services.AddScoped<AdministratorStudentService>();
builder.Services.AddScoped<AdministratorUserAchievementService>();
builder.Services.AddScoped<AdministratorVacancyService>();
builder.Services.AddScoped<BaseAdministratorService>();
builder.Services.AddScoped<BaseEntityService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EmployerService>();
builder.Services.AddScoped<FacultyService>();
builder.Services.AddScoped<FavoriteService>();
builder.Services.AddScoped<InvitationService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<ResumeService>();
builder.Services.AddScoped<SpecialityService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserAchievementService>();
builder.Services.AddScoped<VacancyService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "StudHunter Service");  // check localhost:8080

app.Run();
