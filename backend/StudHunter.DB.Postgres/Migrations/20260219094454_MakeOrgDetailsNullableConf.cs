using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MakeOrgDetailsNullableConf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ogrn",
                table: "OrganizationDetails",
                type: "VARCHAR(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "LegalAddress",
                table: "OrganizationDetails",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Inn",
                table: "OrganizationDetails",
                type: "VARCHAR(12)",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "ActualAddress",
                table: "OrganizationDetails",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ogrn",
                table: "OrganizationDetails",
                type: "VARCHAR(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LegalAddress",
                table: "OrganizationDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Inn",
                table: "OrganizationDetails",
                type: "VARCHAR(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(12)",
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActualAddress",
                table: "OrganizationDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
