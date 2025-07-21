using Microsoft.EntityFrameworkCore;
using StudHunter.API.Services;
using StudHunter.API.Services.AdministratorServices;
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
        Description = "API для управления сущностями в приложении StudHunter."
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
