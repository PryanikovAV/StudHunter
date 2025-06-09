using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudHunter.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "studhunter");

            migrationBuilder.CreateTable(
                name: "AchievementTemplates",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Target = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employers",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    AccreditationStatus = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true),
                    ContactEmail = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    Specialization = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faculties",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialities",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentStatuses",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                schema: "studhunter",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "UUID", nullable: false),
                    AchievementTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    AchievementAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => new { x.UserId, x.AchievementTemplateId });
                    table.ForeignKey(
                        name: "FK_UserAchievements_AchievementTemplates_AchievementTemplateId",
                        column: x => x.AchievementTemplateId,
                        principalSchema: "studhunter",
                        principalTable: "AchievementTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EmployerId = table.Column<Guid>(type: "UUID", nullable: false),
                    Title = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: true),
                    Salary = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacancies_Employers_EmployerId",
                        column: x => x.EmployerId,
                        principalSchema: "studhunter",
                        principalTable: "Employers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FirstName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "DATE", nullable: false),
                    Photo = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true),
                    IsForeign = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CourseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_StudentStatuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "studhunter",
                        principalTable: "StudentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancyCourses",
                schema: "studhunter",
                columns: table => new
                {
                    VacancyId = table.Column<Guid>(type: "UUID", nullable: false),
                    CourseId = table.Column<Guid>(type: "UUID", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyCourses", x => new { x.CourseId, x.VacancyId });
                    table.ForeignKey(
                        name: "FK_VacancyCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "studhunter",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyCourses_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalSchema: "studhunter",
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EmployerId = table.Column<Guid>(type: "UUID", nullable: false),
                    StudentId = table.Column<Guid>(type: "UUID", nullable: false),
                    SenderId = table.Column<Guid>(type: "UUID", nullable: false),
                    Context = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Employers_EmployerId",
                        column: x => x.EmployerId,
                        principalSchema: "studhunter",
                        principalTable: "Employers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "studhunter",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    StudentId = table.Column<Guid>(type: "UUID", nullable: false),
                    Title = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resumes_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "studhunter",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlans",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    StudentId = table.Column<Guid>(type: "UUID", nullable: false),
                    FacultyId = table.Column<Guid>(type: "UUID", nullable: false),
                    SpecialityId = table.Column<Guid>(type: "UUID", nullable: false),
                    StudyForm = table.Column<int>(type: "INTEGER", nullable: false),
                    BeginYear = table.Column<DateOnly>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalSchema: "studhunter",
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Specialities_SpecialityId",
                        column: x => x.SpecialityId,
                        principalSchema: "studhunter",
                        principalTable: "Specialities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "studhunter",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "UUID", nullable: false),
                    TargetId = table.Column<Guid>(type: "UUID", nullable: false),
                    Target = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_Resumes_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "studhunter",
                        principalTable: "Resumes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorites_Vacancies_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "studhunter",
                        principalTable: "Vacancies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SenderId = table.Column<Guid>(type: "UUID", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "UUID", nullable: false),
                    VacancyId = table.Column<Guid>(type: "UUID", nullable: true),
                    ResumeId = table.Column<Guid>(type: "UUID", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "studhunter",
                        principalTable: "Resumes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invitations_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalSchema: "studhunter",
                        principalTable: "Vacancies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudyPlanCourses",
                schema: "studhunter",
                columns: table => new
                {
                    StudyPlanId = table.Column<Guid>(type: "UUID", nullable: false),
                    CourseId = table.Column<Guid>(type: "UUID", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlanCourses", x => new { x.StudyPlanId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_StudyPlanCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "studhunter",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlanCourses_StudyPlans_StudyPlanId",
                        column: x => x.StudyPlanId,
                        principalSchema: "studhunter",
                        principalTable: "StudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "studhunter",
                table: "AchievementTemplates",
                columns: new[] { "Id", "Description", "Name", "Target" },
                values: new object[,]
                {
                    { 1, "Откликнулся на первую вакансию", "Первая попытка", 2 },
                    { 2, "Начал свою первую стажировку", "Новый путь", 2 },
                    { 3, "Успешно окончил стажировку", "Ступень вверх", 2 },
                    { 4, "Полностью заполнил профиль", "Я здесь!", 2 },
                    { 5, "Получил первое трудоустройство", "Первый шаг в карьере", 2 },
                    { 6, "Работаю уже 3 месяца", "Опыт копится III", 2 },
                    { 7, "Работаю уже 6 месяцев", "Опыт копится VI", 2 },
                    { 8, "Работаю уже 9 месяцев", "Опыт копится IX", 2 },
                    { 9, "Работаю уже 12 месяцев", "Опыт копится XII", 2 },
                    { 10, "Получил 10 приглашений от работодателей", "Звезда работодателей", 2 },
                    { 11, "Прошел 3 разных стажировки", "Профи стажировок", 2 },
                    { 12, "Получил первый отзыв от работодателя", "Рекомендации", 2 },
                    { 13, "Заполнил профиль", "Добро пожаловать!", 1 },
                    { 14, "Разместил первую вакансию", "Работодатель мечты", 1 },
                    { 15, "10 студентов откликнулись на вакансии", "Популярность растет", 1 },
                    { 16, "Предоставил стажировку первому студенту", "Первые шаги в наставничестве", 1 },
                    { 17, "Предоставил стажировки для 10 студентов", "Опытный наставник", 1 },
                    { 18, "Взял на работу первого студента", "Первая работа", 1 },
                    { 19, "Нанял 10 студентов", "Крупная компания", 1 },
                    { 20, "Разместил 10 вакансий", "Активный работодатель", 1 },
                    { 21, "Получил 10 положительных отзывов от студентов", "Идеальная репутация", 1 }
                });

            migrationBuilder.InsertData(
                schema: "studhunter",
                table: "StudentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Учусь" },
                    { 2, "В поисках стажировки" },
                    { 3, "В поисках работы" },
                    { 4, "Стажируюсь" },
                    { 5, "Работаю" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementTemplates_Name",
                schema: "studhunter",
                table: "AchievementTemplates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AchievementTemplates_Target",
                schema: "studhunter",
                table: "AchievementTemplates",
                column: "Target");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Name",
                schema: "studhunter",
                table: "Courses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employers_Email",
                schema: "studhunter",
                table: "Employers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_Name",
                schema: "studhunter",
                table: "Faculties",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_TargetId",
                schema: "studhunter",
                table: "Favorites",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_TargetId_Target",
                schema: "studhunter",
                table: "Favorites",
                columns: new[] { "UserId", "TargetId", "Target" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ReceiverId",
                schema: "studhunter",
                table: "Invitations",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ResumeId",
                schema: "studhunter",
                table: "Invitations",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_SenderId_ReceiverId_CreatedAt",
                schema: "studhunter",
                table: "Invitations",
                columns: new[] { "SenderId", "ReceiverId", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_VacancyId",
                schema: "studhunter",
                table: "Invitations",
                column: "VacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_EmployerId_StudentId",
                schema: "studhunter",
                table: "Messages",
                columns: new[] { "EmployerId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_StudentId",
                schema: "studhunter",
                table: "Messages",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_StudentId",
                schema: "studhunter",
                table: "Resumes",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialities_Name",
                schema: "studhunter",
                table: "Specialities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                schema: "studhunter",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_StatusId",
                schema: "studhunter",
                table: "Students",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlanCourses_CourseId",
                schema: "studhunter",
                table: "StudyPlanCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_FacultyId",
                schema: "studhunter",
                table: "StudyPlans",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_SpecialityId",
                schema: "studhunter",
                table: "StudyPlans",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_StudentId",
                schema: "studhunter",
                table: "StudyPlans",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_AchievementTemplateId",
                schema: "studhunter",
                table: "UserAchievements",
                column: "AchievementTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_UserId_AchievementTemplateId",
                schema: "studhunter",
                table: "UserAchievements",
                columns: new[] { "UserId", "AchievementTemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "studhunter",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_EmployerId",
                schema: "studhunter",
                table: "Vacancies",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyCourses_VacancyId",
                schema: "studhunter",
                table: "VacancyCourses",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Invitations",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "StudyPlanCourses",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "UserAchievements",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "VacancyCourses",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Resumes",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "StudyPlans",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "AchievementTemplates",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Courses",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Vacancies",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Faculties",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Specialities",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "Employers",
                schema: "studhunter");

            migrationBuilder.DropTable(
                name: "StudentStatuses",
                schema: "studhunter");
        }
    }
}
