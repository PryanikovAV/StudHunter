using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class NewModelFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployerId",
                schema: "studhunter",
                table: "Favorites",
                type: "UUID",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                schema: "studhunter",
                table: "Favorites",
                type: "UUID",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_EmployerId",
                schema: "studhunter",
                table: "Favorites",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_StudentId",
                schema: "studhunter",
                table: "Favorites",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_EmployerId",
                schema: "studhunter",
                table: "Favorites",
                columns: new[] { "UserId", "EmployerId" },
                unique: true,
                filter: "\"EmployerId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_StudentId",
                schema: "studhunter",
                table: "Favorites",
                columns: new[] { "UserId", "StudentId" },
                unique: true,
                filter: "\"StudentId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Employers_EmployerId",
                schema: "studhunter",
                table: "Favorites",
                column: "EmployerId",
                principalSchema: "studhunter",
                principalTable: "Employers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Students_StudentId",
                schema: "studhunter",
                table: "Favorites",
                column: "StudentId",
                principalSchema: "studhunter",
                principalTable: "Students",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Employers_EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Students_StudentId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_StudentId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_StudentId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                schema: "studhunter",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "studhunter",
                table: "Favorites");
        }
    }
}
