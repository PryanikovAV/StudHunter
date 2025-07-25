using Microsoft.EntityFrameworkCore;
using StudHunter.API.Services;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudHunterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        var jwtKey = builder.Configuration["Jwt:Key"];
//        if (string.IsNullOrEmpty(jwtKey))
//            throw new InvalidOperationException("JWT Key is not configured in appsettings.json");

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
//        };
//    });

//builder.Services.AddAuthorization();

builder.Services.AddScoped<AdminAchievementTemplateService>();
builder.Services.AddScoped<AdminCourseService>();
builder.Services.AddScoped<AdminEmployerService>();
builder.Services.AddScoped<AdminFacultyService>();
builder.Services.AddScoped<AdminFavoriteService>();
builder.Services.AddScoped<AdminInvitationService>();
builder.Services.AddScoped<AdminMessagesService>();
builder.Services.AddScoped<AdminResumeService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AdminSpecialityService>();
builder.Services.AddScoped<AdminStudentService>();
builder.Services.AddScoped<AdminUserAchievementService>();
builder.Services.AddScoped<AdminVacancyService>();
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StudHunter API",
        Version = "v1",
        Description = "API ��� ���������� ���������� � ���������� StudHunter."
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudHunter API v1");
        c.RoutePrefix = "swagger";
    });
}

//app.UseAuthentication();
//app.UseAuthorization();

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "StudHunter Service");

app.Run();
