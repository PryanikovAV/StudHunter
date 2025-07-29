using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudHunter.DB.Postgres.Migrations
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
                    Id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrderNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IconUrl = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    Target = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Email = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    ContactEmail = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    FirstName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    AdminLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
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
                    Id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Email = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    ContactEmail = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    AccreditationStatus = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
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
                name: "Messages",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SenderId = table.Column<Guid>(type: "UUID", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "UUID", nullable: false),
                    Context = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
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
                name: "UserAchievements",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "UUID", nullable: false),
                    AchievementTemplateId = table.Column<Guid>(type: "UUID", nullable: false),
                    AchievementAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.Id);
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
                    Salary = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false)
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
                    Id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Email = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    ContactEmail = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    FirstName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "DATE", nullable: false),
                    Photo = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    IsForeign = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: true)
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
                        onDelete: ReferentialAction.SetNull);
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
                        principalColumn: "Id");
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
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false)
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
                    CourseNumber = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
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
                    VacancyId = table.Column<Guid>(type: "UUID", nullable: true),
                    ResumeId = table.Column<Guid>(type: "UUID", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "studhunter",
                        principalTable: "Resumes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorites_Vacancies_VacancyId",
                        column: x => x.VacancyId,
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
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
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
                name: "IX_AchievementTemplates_OrderNumber",
                schema: "studhunter",
                table: "AchievementTemplates",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_Email",
                schema: "studhunter",
                table: "Administrators",
                column: "Email",
                unique: true);

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
                name: "IX_Favorites_ResumeId",
                schema: "studhunter",
                table: "Favorites",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_ResumeId",
                schema: "studhunter",
                table: "Favorites",
                columns: new[] { "UserId", "ResumeId" },
                unique: true,
                filter: "\"ResumeId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_VacancyId",
                schema: "studhunter",
                table: "Favorites",
                columns: new[] { "UserId", "VacancyId" },
                unique: true,
                filter: "\"VacancyId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_VacancyId",
                schema: "studhunter",
                table: "Favorites",
                column: "VacancyId");

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
                name: "IX_Invitations_SenderId_ReceiverId_ResumeId",
                schema: "studhunter",
                table: "Invitations",
                columns: new[] { "SenderId", "ReceiverId", "ResumeId" },
                unique: true,
                filter: "\"ResumeId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_SenderId_ReceiverId_VacancyId",
                schema: "studhunter",
                table: "Invitations",
                columns: new[] { "SenderId", "ReceiverId", "VacancyId" },
                unique: true,
                filter: "\"VacancyId\" is not NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_VacancyId",
                schema: "studhunter",
                table: "Invitations",
                column: "VacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                schema: "studhunter",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                schema: "studhunter",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentAt",
                schema: "studhunter",
                table: "Messages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_CreatedAt",
                schema: "studhunter",
                table: "Resumes",
                column: "CreatedAt");

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
                name: "Administrators",
                schema: "studhunter");

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
