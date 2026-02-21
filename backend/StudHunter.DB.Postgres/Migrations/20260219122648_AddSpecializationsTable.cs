using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecializationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_Specialization_SpecializationId",
                table: "Employers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialization",
                table: "Specialization");

            migrationBuilder.RenameTable(
                name: "Specialization",
                newName: "Specializations");

            migrationBuilder.RenameIndex(
                name: "IX_Specialization_Name",
                table: "Specializations",
                newName: "IX_Specializations_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specializations",
                table: "Specializations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_Specializations_SpecializationId",
                table: "Employers",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_Specializations_SpecializationId",
                table: "Employers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specializations",
                table: "Specializations");

            migrationBuilder.RenameTable(
                name: "Specializations",
                newName: "Specialization");

            migrationBuilder.RenameIndex(
                name: "IX_Specializations_Name",
                table: "Specialization",
                newName: "IX_Specialization_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialization",
                table: "Specialization",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_Specialization_SpecializationId",
                table: "Employers",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
