using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RefactorInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invitations_SenderId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Unique_Active_General_Offer",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Unique_Active_Invitation",
                table: "Invitations");

            migrationBuilder.RenameColumn(
                name: "SnapshotSenderName",
                table: "Invitations",
                newName: "SnapshotStudentName");

            migrationBuilder.RenameColumn(
                name: "SnapshotReceiverName",
                table: "Invitations",
                newName: "SnapshotEmployerName");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Invitations",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Invitations",
                newName: "EmployerId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_ReceiverId",
                table: "Invitations",
                newName: "IX_Invitations_EmployerId");

            migrationBuilder.AlterColumn<Guid>(
                name: "VacancyId",
                table: "Invitations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "UUID",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Invitations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "UUID",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unique_Active_Invitation",
                table: "Invitations",
                columns: new[] { "StudentId", "EmployerId", "VacancyId", "Type" },
                unique: true,
                filter: "\"Status\" = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Employers_EmployerId",
                table: "Invitations",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Students_StudentId",
                table: "Invitations",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Employers_EmployerId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Students_StudentId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Unique_Active_Invitation",
                table: "Invitations");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Invitations",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "SnapshotStudentName",
                table: "Invitations",
                newName: "SnapshotSenderName");

            migrationBuilder.RenameColumn(
                name: "SnapshotEmployerName",
                table: "Invitations",
                newName: "SnapshotReceiverName");

            migrationBuilder.RenameColumn(
                name: "EmployerId",
                table: "Invitations",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_EmployerId",
                table: "Invitations",
                newName: "IX_Invitations_ReceiverId");

            migrationBuilder.AlterColumn<Guid>(
                name: "VacancyId",
                table: "Invitations",
                type: "UUID",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Invitations",
                type: "UUID",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_SenderId",
                table: "Invitations",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Unique_Active_General_Offer",
                table: "Invitations",
                columns: new[] { "SenderId", "ReceiverId", "Status" },
                unique: true,
                filter: "\"Status\" = 0 AND \"VacancyId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Unique_Active_Invitation",
                table: "Invitations",
                columns: new[] { "SenderId", "ReceiverId", "VacancyId", "Status" },
                unique: true,
                filter: "\"Status\" = 0");
        }
    }
}
