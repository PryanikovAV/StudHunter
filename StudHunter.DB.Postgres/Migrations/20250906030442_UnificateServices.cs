using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UnificateServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Resumes_ResumeId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Students_StudentId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlanCourses_Courses_CourseId",
                schema: "studhunter",
                table: "StudyPlanCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancyCourses_Courses_CourseId",
                schema: "studhunter",
                table: "VacancyCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancyCourses_Vacancies_VacancyId",
                schema: "studhunter",
                table: "VacancyCourses");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_ResumeId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_StudentId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_ResumeId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_VacancyId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "BeginYear",
                schema: "studhunter",
                table: "StudyPlans");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "ResumeId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "AdminLevel",
                schema: "studhunter",
                table: "Administrators");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "studhunter",
                table: "Vacancies",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Vacancies",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Vacancies",
                type: "TIMESTAMPTZ",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AchievementAt",
                schema: "studhunter",
                table: "UserAchievements",
                type: "TIMESTAMPTZ",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "SpecialityId",
                schema: "studhunter",
                table: "StudyPlans",
                type: "UUID",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "UUID",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "FacultyId",
                schema: "studhunter",
                table: "StudyPlans",
                type: "UUID",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "UUID",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "studhunter",
                table: "StudyPlans",
                type: "TIMESTAMPTZ",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "studhunter",
                table: "StudyPlans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Students",
                type: "TIMESTAMPTZ",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "studhunter",
                table: "Resumes",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Resumes",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Resumes",
                type: "TIMESTAMPTZ",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                schema: "studhunter",
                table: "Messages",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "studhunter",
                table: "Invitations",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Invitations",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AddedAt",
                schema: "studhunter",
                table: "Favorites",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Employers",
                type: "TIMESTAMPTZ",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMessageAt",
                schema: "studhunter",
                table: "Chats",
                type: "TIMESTAMPTZ",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Chats",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Administrators",
                type: "TIMESTAMPTZ",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                schema: "studhunter",
                table: "Administrators",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_EmployerId",
                schema: "studhunter",
                table: "Favorites",
                column: "EmployerId",
                filter: "\"EmployerId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_StudentId",
                schema: "studhunter",
                table: "Favorites",
                column: "StudentId",
                filter: "\"StudentId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                schema: "studhunter",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_VacancyId",
                schema: "studhunter",
                table: "Favorites",
                column: "VacancyId",
                filter: "\"EmployerId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Students_StudentId",
                schema: "studhunter",
                table: "Favorites",
                column: "StudentId",
                principalSchema: "studhunter",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Favorites",
                column: "VacancyId",
                principalSchema: "studhunter",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlanCourses_Courses_CourseId",
                schema: "studhunter",
                table: "StudyPlanCourses",
                column: "CourseId",
                principalSchema: "studhunter",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VacancyCourses_Courses_CourseId",
                schema: "studhunter",
                table: "VacancyCourses",
                column: "CourseId",
                principalSchema: "studhunter",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VacancyCourses_Vacancies_VacancyId",
                schema: "studhunter",
                table: "VacancyCourses",
                column: "VacancyId",
                principalSchema: "studhunter",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Students_StudentId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlanCourses_Courses_CourseId",
                schema: "studhunter",
                table: "StudyPlanCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancyCourses_Courses_CourseId",
                schema: "studhunter",
                table: "VacancyCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancyCourses_Vacancies_VacancyId",
                schema: "studhunter",
                table: "VacancyCourses");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_StudentId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_VacancyId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "studhunter",
                table: "StudyPlans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "studhunter",
                table: "StudyPlans");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "studhunter",
                table: "Administrators");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                schema: "studhunter",
                table: "Administrators");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "studhunter",
                table: "Vacancies",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Vacancies",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AchievementAt",
                schema: "studhunter",
                table: "UserAchievements",
                type: "TIMESTAMP",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ");

            migrationBuilder.AlterColumn<Guid>(
                name: "SpecialityId",
                schema: "studhunter",
                table: "StudyPlans",
                type: "UUID",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "UUID");

            migrationBuilder.AlterColumn<Guid>(
                name: "FacultyId",
                schema: "studhunter",
                table: "StudyPlans",
                type: "UUID",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "UUID");

            migrationBuilder.AddColumn<DateOnly>(
                name: "BeginYear",
                schema: "studhunter",
                table: "StudyPlans",
                type: "DATE",
                nullable: false,
                defaultValue: new DateOnly(2025, 8, 16));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "studhunter",
                table: "Resumes",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Resumes",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                schema: "studhunter",
                table: "Messages",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "studhunter",
                table: "Invitations",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Invitations",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "studhunter",
                table: "Invitations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AddedAt",
                schema: "studhunter",
                table: "Favorites",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "ResumeId",
                schema: "studhunter",
                table: "Favorites",
                type: "UUID",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMessageAt",
                schema: "studhunter",
                table: "Chats",
                type: "TIMESTAMP",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Chats",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "AdminLevel",
                schema: "studhunter",
                table: "Administrators",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_EmployerId",
                schema: "studhunter",
                table: "Favorites",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ResumeId",
                schema: "studhunter",
                table: "Favorites",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_StudentId",
                schema: "studhunter",
                table: "Favorites",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_ResumeId",
                schema: "studhunter",
                table: "Favorites",
                columns: new[] { "UserId", "ResumeId" },
                unique: true,
                filter: "\"ResumeId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_VacancyId",
                schema: "studhunter",
                table: "Favorites",
                column: "VacancyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Resumes_ResumeId",
                schema: "studhunter",
                table: "Favorites",
                column: "ResumeId",
                principalSchema: "studhunter",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Students_StudentId",
                schema: "studhunter",
                table: "Favorites",
                column: "StudentId",
                principalSchema: "studhunter",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Favorites",
                column: "VacancyId",
                principalSchema: "studhunter",
                principalTable: "Vacancies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlanCourses_Courses_CourseId",
                schema: "studhunter",
                table: "StudyPlanCourses",
                column: "CourseId",
                principalSchema: "studhunter",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VacancyCourses_Courses_CourseId",
                schema: "studhunter",
                table: "VacancyCourses",
                column: "CourseId",
                principalSchema: "studhunter",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VacancyCourses_Vacancies_VacancyId",
                schema: "studhunter",
                table: "VacancyCourses",
                column: "VacancyId",
                principalSchema: "studhunter",
                principalTable: "Vacancies",
                principalColumn: "Id");
        }
    }
}
