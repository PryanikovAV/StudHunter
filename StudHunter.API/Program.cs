using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudHunter.API.Common;
using StudHunter.API.Services;
using StudHunter.API.Services.AdminServices;
using StudHunter.DB.Postgres;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddDbContext<StudHunterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddScoped<AdditionalSkillService>();
builder.Services.AddScoped<AdminAchievementTemplateService>();
builder.Services.AddScoped<AdminAdditionalSkillService>();
builder.Services.AddScoped<AdminChatService>();
builder.Services.AddScoped<AdminCourseService>();
builder.Services.AddScoped<AdminEmployerService>();
builder.Services.AddScoped<AdminFacultyService>();
builder.Services.AddScoped<AdminFavoriteService>();
builder.Services.AddScoped<AdminInvitationService>();
builder.Services.AddScoped<AdminMessageService>();
builder.Services.AddScoped<AdminResumeService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AdminSpecialityService>();
builder.Services.AddScoped<AdminStudentService>();
builder.Services.AddScoped<AdminStudyPlanService>();
builder.Services.AddScoped<AdminUserAchievementService>();
builder.Services.AddScoped<AdminVacancyService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EmployerService>();
builder.Services.AddScoped<FacultyService>();
builder.Services.AddScoped<FavoriteService>();
builder.Services.AddScoped<InvitationService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<ResumeService>();
builder.Services.AddScoped<SpecialityService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<StudyPlanService>();
builder.Services.AddScoped<UserAchievementService>();
builder.Services.AddScoped<VacancyService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StudHunter API",
        Version = "v1",
        Description = "API for managing entities in the StudHunter application."
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field (e.g. 'Bearer {token}'",
        Name = "authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StudHunterDbContext>();
    FillAchievements.SeedAchievements(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudHunter API v1");
        c.RoutePrefix = "swagger";
    });
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "StudHunter Service");

app.Run();
