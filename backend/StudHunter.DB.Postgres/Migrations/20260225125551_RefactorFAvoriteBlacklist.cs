using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RefactorFAvoriteBlacklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Resumes_ResumeId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_ResumeId",
                table: "Favorites");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Favorite_AtLeastOneTarget",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "ResumeId",
                table: "Favorites",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_ResumeId",
                table: "Favorites",
                newName: "IX_Favorites_StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_StudentId",
                table: "Favorites",
                columns: new[] { "UserId", "StudentId" },
                unique: true,
                filter: "\"StudentId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Favorite_AtLeastOneTarget",
                table: "Favorites",
                sql: "\"VacancyId\" IS NOT NULL OR \"EmployerId\" IS NOT NULL OR \"StudentId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Students_StudentId",
                table: "Favorites",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Students_StudentId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_StudentId",
                table: "Favorites");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Favorite_AtLeastOneTarget",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Favorites",
                newName: "ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_StudentId",
                table: "Favorites",
                newName: "IX_Favorites_ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_ResumeId",
                table: "Favorites",
                columns: new[] { "UserId", "ResumeId" },
                unique: true,
                filter: "\"ResumeId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Favorite_AtLeastOneTarget",
                table: "Favorites",
                sql: "\"VacancyId\" IS NOT NULL OR \"EmployerId\" IS NOT NULL OR \"ResumeId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Resumes_ResumeId",
                table: "Favorites",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
