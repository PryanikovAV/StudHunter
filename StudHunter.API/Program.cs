using Microsoft.EntityFrameworkCore;
using StudHunter.API.Services;
using StudHunter.DB.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudHunterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<AdministratorService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EmployerService>();
builder.Services.AddScoped<FacultyService>();
builder.Services.AddScoped<FavoriteService>();
builder.Services.AddScoped<ResumeService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<VacancyService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "StudHunter Service");  // check localhost:8080

app.Run();