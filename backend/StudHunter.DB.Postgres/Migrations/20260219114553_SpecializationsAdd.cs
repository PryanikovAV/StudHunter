using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SpecializationsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Employers");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecializationId",
                table: "Employers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Specialization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialization", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Specialization",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("12345678-9abc-4b2a-9e1d-3b5a1f2c4d5e"), "Розничная торговля" },
                    { new Guid("87654321-9abc-4b2a-9e1d-3b5a1f2c4d5e"), "Гостиницы, рестораны, общепит" },
                    { new Guid("abcdefab-9abc-4b2a-9e1d-3b5a1f2c4d5e"), "Медицина, фармацевтика, аптеки" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employers_SpecializationId",
                table: "Employers",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Specialization_Name",
                table: "Specialization",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_Specialization_SpecializationId",
                table: "Employers",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_Specialization_SpecializationId",
                table: "Employers");

            migrationBuilder.DropTable(
                name: "Specialization");

            migrationBuilder.DropIndex(
                name: "IX_Employers_SpecializationId",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Employers");

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Employers",
                type: "VARCHAR(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
