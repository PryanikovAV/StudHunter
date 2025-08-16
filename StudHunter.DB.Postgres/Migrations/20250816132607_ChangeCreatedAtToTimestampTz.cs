using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCreatedAtToTimestampTz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Employers_EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Resumes_ResumeId",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentStatuses_StatusId",
                schema: "studhunter",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_Faculties_FacultyId",
                schema: "studhunter",
                table: "StudyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_Specialities_SpecialityId",
                schema: "studhunter",
                table: "StudyPlans");

            migrationBuilder.DropTable(
                name: "StudentStatuses",
                schema: "studhunter");

            migrationBuilder.DropIndex(
                name: "IX_Students_StatusId",
                schema: "studhunter",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_SenderId_ReceiverId_ResumeId",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_SenderId_ReceiverId_VacancyId",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_AchievementTemplates_OrderNumber",
                schema: "studhunter",
                table: "AchievementTemplates");

            migrationBuilder.DropColumn(
                name: "StatusId",
                schema: "studhunter",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "studhunter",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Message",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "studhunter",
                table: "AchievementTemplates");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                schema: "studhunter",
                table: "Messages",
                newName: "ChatId");

            migrationBuilder.RenameColumn(
                name: "Context",
                schema: "studhunter",
                table: "Messages",
                newName: "Content");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReceiverId",
                schema: "studhunter",
                table: "Messages",
                newName: "IX_Messages_ChatId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BeginYear",
                schema: "studhunter",
                table: "StudyPlans",
                type: "DATE",
                nullable: false,
                defaultValue: new DateOnly(2025, 8, 16),
                oldClrType: typeof(DateOnly),
                oldType: "DATE",
                oldDefaultValue: new DateOnly(2025, 8, 13));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Students",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "studhunter",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "InvitationId",
                schema: "studhunter",
                table: "Messages",
                type: "UUID",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "studhunter",
                table: "Messages",
                type: "UUID",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Employers",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Administrators",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "Chats",
                schema: "studhunter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    User1Id = table.Column<Guid>(type: "UUID", nullable: false),
                    User2Id = table.Column<Guid>(type: "UUID", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastMessageAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_InvitationId",
                schema: "studhunter",
                table: "Messages",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                schema: "studhunter",
                table: "Messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_SenderId",
                schema: "studhunter",
                table: "Invitations",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementTemplates_Name",
                schema: "studhunter",
                table: "AchievementTemplates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_User1Id_User2Id",
                schema: "studhunter",
                table: "Chats",
                columns: new[] { "User1Id", "User2Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_User2Id",
                schema: "studhunter",
                table: "Chats",
                column: "User2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Employers_EmployerId",
                schema: "studhunter",
                table: "Favorites",
                column: "EmployerId",
                principalSchema: "studhunter",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Resumes_ResumeId",
                schema: "studhunter",
                table: "Invitations",
                column: "ResumeId",
                principalSchema: "studhunter",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Invitations",
                column: "VacancyId",
                principalSchema: "studhunter",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                schema: "studhunter",
                table: "Messages",
                column: "ChatId",
                principalSchema: "studhunter",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Invitations_InvitationId",
                schema: "studhunter",
                table: "Messages",
                column: "InvitationId",
                principalSchema: "studhunter",
                principalTable: "Invitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_Faculties_FacultyId",
                schema: "studhunter",
                table: "StudyPlans",
                column: "FacultyId",
                principalSchema: "studhunter",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_Specialities_SpecialityId",
                schema: "studhunter",
                table: "StudyPlans",
                column: "SpecialityId",
                principalSchema: "studhunter",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Employers_EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Resumes_ResumeId",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                schema: "studhunter",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Invitations_InvitationId",
                schema: "studhunter",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_Faculties_FacultyId",
                schema: "studhunter",
                table: "StudyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_Specialities_SpecialityId",
                schema: "studhunter",
                table: "StudyPlans");

            migrationBuilder.DropTable(
                name: "Chats",
                schema: "studhunter");

            migrationBuilder.DropIndex(
                name: "IX_Messages_InvitationId",
                schema: "studhunter",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserId",
                schema: "studhunter",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_SenderId",
                schema: "studhunter",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_AchievementTemplates_Name",
                schema: "studhunter",
                table: "AchievementTemplates");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "studhunter",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "InvitationId",
                schema: "studhunter",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "studhunter",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "studhunter",
                table: "Messages",
                newName: "Context");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                schema: "studhunter",
                table: "Messages",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatId",
                schema: "studhunter",
                table: "Messages",
                newName: "IX_Messages_ReceiverId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BeginYear",
                schema: "studhunter",
                table: "StudyPlans",
                type: "DATE",
                nullable: false,
                defaultValue: new DateOnly(2025, 8, 13),
                oldClrType: typeof(DateOnly),
                oldType: "DATE",
                oldDefaultValue: new DateOnly(2025, 8, 16));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Students",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                schema: "studhunter",
                table: "Students",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                schema: "studhunter",
                table: "Students",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                schema: "studhunter",
                table: "Invitations",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Employers",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "studhunter",
                table: "Administrators",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "studhunter",
                table: "AchievementTemplates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_Students_StatusId",
                schema: "studhunter",
                table: "Students",
                column: "StatusId");

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
                name: "IX_AchievementTemplates_OrderNumber",
                schema: "studhunter",
                table: "AchievementTemplates",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Employers_EmployerId",
                schema: "studhunter",
                table: "Favorites",
                column: "EmployerId",
                principalSchema: "studhunter",
                principalTable: "Employers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Resumes_ResumeId",
                schema: "studhunter",
                table: "Invitations",
                column: "ResumeId",
                principalSchema: "studhunter",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Vacancies_VacancyId",
                schema: "studhunter",
                table: "Invitations",
                column: "VacancyId",
                principalSchema: "studhunter",
                principalTable: "Vacancies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentStatuses_StatusId",
                schema: "studhunter",
                table: "Students",
                column: "StatusId",
                principalSchema: "studhunter",
                principalTable: "StudentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_Faculties_FacultyId",
                schema: "studhunter",
                table: "StudyPlans",
                column: "FacultyId",
                principalSchema: "studhunter",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_Specialities_SpecialityId",
                schema: "studhunter",
                table: "StudyPlans",
                column: "SpecialityId",
                principalSchema: "studhunter",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
