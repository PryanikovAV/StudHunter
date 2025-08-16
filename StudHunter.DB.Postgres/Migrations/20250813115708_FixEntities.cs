using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.DB.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FixEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StudyForm",
                schema: "studhunter",
                table: "StudyPlans",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER");

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

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BeginYear",
                schema: "studhunter",
                table: "StudyPlans",
                type: "DATE",
                nullable: false,
                defaultValue: new DateOnly(2025, 8, 13),
                oldClrType: typeof(DateOnly),
                oldType: "DATE");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "studhunter",
                table: "StudyPlans",
                type: "UUID",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                schema: "studhunter",
                table: "Students",
                type: "DATE",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "DATE");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                schema: "studhunter",
                table: "Students",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "studhunter",
                table: "Students");

            migrationBuilder.AlterColumn<int>(
                name: "StudyForm",
                schema: "studhunter",
                table: "StudyPlans",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

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

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BeginYear",
                schema: "studhunter",
                table: "StudyPlans",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "DATE",
                oldDefaultValue: new DateOnly(2025, 8, 13));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "studhunter",
                table: "StudyPlans",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "UUID",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                schema: "studhunter",
                table: "Students",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "DATE",
                oldDefaultValue: new DateOnly(1, 1, 1));
        }
    }
}
